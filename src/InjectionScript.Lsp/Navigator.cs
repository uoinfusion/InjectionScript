using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Lsp
{
    public sealed class Navigator
    {
        private readonly IInjectionWorkspace workspace;

        public Navigator(IInjectionWorkspace workspace)
        {
            this.workspace = workspace;
        }

        public Location GetDefinition(Uri file, int line, int column)
        {
            if (!workspace.TryGetDocument(file, out var fileContent))
                return null;

            var result = GetDefinition(fileContent, line, column);
            if (result != null)
                result.Uri = file;

            return result;
        }

        public Location GetDefinition(string fileContent, int line, int column)
        {
            string fileName = "test.sc";
            var runtime = new InjectionRuntime();
            runtime.Load(fileContent, fileName);

            var rule = FindRule(runtime.CurrentFileSyntax, line, column);
            if (rule == null)
                return null;

            if (rule is injectionParser.CallContext call)
            {
                var subrutineName = call.SYMBOL().GetText();
                var argumentCount = call.argumentList()?.arguments()?.argument()?.Length ?? 0;
                if (runtime.Metadata.TryGetSubrutine(subrutineName, argumentCount, out var subrutineDefinition))
                {
                    var subrutineDefName = subrutineDefinition.Syntax.subrutineName();
                    return new Location()
                    {
                        Range = new Range(new Position(subrutineDefName.Start.Line, subrutineDefName.Start.Column + 1),
                            new Position(subrutineDefName.Stop.Line, subrutineDefName.Start.Column + 1 + subrutineName.Length))
                    };
                }
            }
            else if (rule is injectionParser.LvalueContext lvalue)
            {
                var subrutine = GetSubrutine(lvalue);
                if (subrutine != null)
                {
                    var definitions = new Dictionary<string, ParserRuleContext>(StringComparer.OrdinalIgnoreCase);
                    var collector = new SymbolDefinitionsCollector(definitions);
                    collector.Visit(subrutine);

                    string name = lvalue.SYMBOL()?.GetText() ?? lvalue.indexedSymbol().SYMBOL().GetText();
                    if (definitions.TryGetValue(name, out var definitionRule))
                    {
                        var endColumn = definitionRule.Start.Column + 1 + definitionRule.GetText().Length;
                        return new Location()
                        {
                            Range = new Range(new Position(definitionRule.Start.Line, definitionRule.Start.Column + 1),
                                new Position(definitionRule.Start.Line, endColumn))
                        };
                    }
                }
            }
            else if (rule is injectionParser.GotoContext gotoContext)
            {
                var subrutine = GetSubrutine(gotoContext);
                if (subrutine != null)
                {
                    var definitions = new Dictionary<string, ParserRuleContext>(StringComparer.OrdinalIgnoreCase);
                    var collector = new SymbolDefinitionsCollector(definitions);
                    collector.Visit(subrutine);

                    string name = gotoContext.SYMBOL().GetText();
                    if (definitions.TryGetValue(name, out var definitionRule))
                    {
                        var endColumn = definitionRule.Start.Column + definitionRule.GetText().Length - 2;
                        return new Location()
                        {
                            Range = new Range(new Position(definitionRule.Start.Line, definitionRule.Start.Column + 1),
                                new Position(definitionRule.Start.Line, endColumn))
                        };
                    }
                }
            }
            else if (rule is injectionParser.OperandContext operand)
            {
                var subrutine = GetSubrutine(operand);
                if (subrutine != null)
                {
                    var definitions = new Dictionary<string, ParserRuleContext>(StringComparer.OrdinalIgnoreCase);
                    var collector = new SymbolDefinitionsCollector(definitions);
                    collector.Visit(subrutine);

                    string name = operand.SYMBOL()?.GetText() ?? operand.indexedSymbol().SYMBOL().GetText();
                    if (definitions.TryGetValue(name, out var definitionRule))
                    {
                        var endColumn = definitionRule.Start.Column + 1 + definitionRule.GetText().Length;
                        return new Location()
                        {
                            Range = new Range(new Position(definitionRule.Start.Line, definitionRule.Start.Column + 1),
                                new Position(definitionRule.Start.Line, endColumn))
                        };
                    }
                }
            }
            return null;
        }

        private class SymbolDefinitionsCollector : injectionBaseVisitor<bool>
        {
            private readonly Dictionary<string, ParserRuleContext> definitions;

            public SymbolDefinitionsCollector(Dictionary<string, ParserRuleContext> definitions)
            {
                this.definitions = definitions;
            }

            public override bool VisitVarDef([NotNull] injectionParser.VarDefContext context)
            {
                var name = context.SYMBOL()?.GetText() ?? context.assignment()?.lvalue()?.SYMBOL()?.GetText();
                if (name != null)
                    definitions[name] = context;

                return base.VisitVarDef(context);
            }

            public override bool VisitDimDef([NotNull] injectionParser.DimDefContext context)
            {
                var name = context.SYMBOL()?.GetText();
                if (name != null)
                    definitions[name] = context;

                return base.VisitDimDef(context);
            }

            public override bool VisitLabel([NotNull] injectionParser.LabelContext context)
            {
                var name = context.SYMBOL()?.GetText();
                if (name != null)
                    definitions[name] = context;

                return base.VisitLabel(context);
            }
        }

        private injectionParser.SubrutineContext GetSubrutine(IRuleNode node)
        {
            while (node != null)
            {
                if (node is injectionParser.SubrutineContext subrutine)
                    return subrutine;

                node = node.Parent;
            }

            return null;
        }

        private ParserRuleContext FindRule(ParserRuleContext parent, int line, int column)
        {
            if (parent.Start.Line == line && parent.Stop.Line == line)
            {
                if (parent is injectionParser.CallContext call)
                {
                    var endColumn = call.Start.Column + call.SYMBOL().GetText().Length + 1;
                    if (column >= call.Start.Column && column <= endColumn)
                        return parent;
                }
                else if (parent is injectionParser.LvalueContext lvalue)
                {
                    var len = lvalue.SYMBOL()?.GetText()?.Length
                        ?? lvalue.indexedSymbol().GetText().Length;
                    var endColumn = lvalue.Start.Column + len + 1;
                    if (column >= lvalue.Start.Column && column <= endColumn)
                        return parent;
                }
                else if (parent is injectionParser.GotoContext gotoContext)
                {
                    var startColumn = gotoContext.SYMBOL().Symbol.Column;
                    var endColumn = startColumn + gotoContext.SYMBOL().GetText().Length + 1;
                    if (column >= startColumn && column <= endColumn)
                        return parent;
                }
                else if (parent is injectionParser.OperandContext operand)
                {
                    if (operand.SYMBOL() != null)
                    {
                        var startColumn = operand.SYMBOL().Symbol.Column;
                        var endColumn = startColumn + operand.SYMBOL().GetText().Length + 1;
                        if (column >= startColumn && column <= endColumn)
                            return parent;
                    }
                    else if (operand.indexedSymbol() != null)
                    {
                        var startColumn = operand.indexedSymbol().Start.Column;
                        var endColumn = startColumn + operand.indexedSymbol().GetText().Length + 1;
                        if (column >= startColumn && column <= endColumn)
                            return parent;
                    }
                }
            }

            foreach (var child in parent.children)
            {
                if (child is ParserRuleContext childRule)
                {
                    var found = FindRule(childRule, line, column);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }
    }
}
