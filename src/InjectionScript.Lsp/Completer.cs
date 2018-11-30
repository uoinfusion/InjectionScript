using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Lsp
{
    public class Completer
    {
        private static readonly CompletionItem[] statementKeywords = new[]
        {
            new CompletionItem() { Label = "if", InsertText = "if", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "else", InsertText = "else", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "end if", InsertText = "end if", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "repeat", InsertText = "repeat", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "until", InsertText = "until", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "for", InsertText = "for", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "next", InsertText = "next", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "while", InsertText = "while", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "wend", InsertText = "wend", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "return", InsertText = "return", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "goto", InsertText = "goto", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "dim", InsertText = "dim", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "var", InsertText = "var", Kind = CompletionItemKind.Keyword },
        };

        public CompletionList GetCompletions(injectionParser.FileContext fileSyntax, Metadata metadata, int line, int column)
        {
            var subrutineSyntax = GetSubrutine(fileSyntax, line, column);
            if (subrutineSyntax == null)
                return new CompletionList(new[]
                {
                    new CompletionItem() { Label = "sub", InsertText = "sub", Kind = CompletionItemKind.Keyword },
                    new CompletionItem() { Label = "end sub", InsertText = "end sub", Kind = CompletionItemKind.Keyword },
                });

            var result = GetContext(fileSyntax, line, column);
            if (!result.HasValue)
                return new CompletionList(true);

            var context = result.Value;

            var namespaceSuggestions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var completions = Enumerable.Empty<CompletionItem>();

            if (context.AllowedSuggestions.HasFlag(SuggestionKind.Keywords) && !context.StartsWith("UO."))
                completions = completions.Concat(statementKeywords);

            if (context.AllowedSuggestions.HasFlag(SuggestionKind.Variables) && !context.StartsWith("UO."))
            {
                var collector = new VariableDefinitionCollector(line, context.Prefix);
                collector.Visit(subrutineSyntax);

                completions = completions.Concat(collector.VariableNames.Select(CreateVariableCompletion));
            }

            if (context.AllowedSuggestions.HasFlag(SuggestionKind.Namespace) && !context.StartsWith("UO."))
                namespaceSuggestions.Add("UO");

            if (context.AllowedSuggestions.HasFlag(SuggestionKind.Namespace))
            {
                var subrutineCompletions = metadata.NativeSubrutines.Where(subrutine => context.IsSameNamespace(subrutine.Name))
                    .GroupBy(subrutine => subrutine.Name)
                    .Select(group => CreateSubrutineCompletion(group.First().Name));
                completions = completions.Concat(subrutineCompletions);

                if (!context.HasNamespace)
                {
                    subrutineCompletions = metadata.Subrutines.GroupBy(subrutine => subrutine.Name)
                        .Select(group => CreateSubrutineCompletion(group.First().Name));
                    completions = completions.Concat(subrutineCompletions);
                }
            }

            completions = completions
                .Concat(namespaceSuggestions.Select(x => CreateNamespaceCompletion(x)));

            return new CompletionList(completions.ToArray());
        }

        private CompletionItem CreateVariableCompletion(string variableName) =>
            new CompletionItem() { Label = variableName, InsertText = variableName, Kind = CompletionItemKind.Variable };


        private CompletionItem CreateSubrutineCompletion(string subrutineName)
        {
            if (subrutineName.StartsWith("UO."))
                subrutineName = subrutineName.Substring(3);

            return new CompletionItem() { Label = subrutineName, InsertText = subrutineName, Kind = CompletionItemKind.Function };
        }

        private CompletionItem CreateNamespaceCompletion(string ns)
            => new CompletionItem() { Label = ns, InsertText = ns, Kind = CompletionItemKind.Class };

        public CompletionList GetCompletions(string fileContent, int line, int column)
        {
            var runtime = new InjectionRuntime();
            runtime.Load(fileContent, "test.sc");

            return GetCompletions(runtime.CurrentFileSyntax, runtime.Metadata, line, column);
        }

        private injectionParser.SubrutineContext GetSubrutine(injectionParser.FileContext fileSyntax, int line, int column)
        {
            for (var i = 0; i < fileSyntax.ChildCount; i++)
            {
                var child = fileSyntax.GetChild(i) as injectionParser.SubrutineContext;
                if (child != null && line >= child.Start.Line && line <= child.Stop.Line)
                {
                    return child;
                }
            }

            return null;
        }

        private SuggestionContext? GetContext(IParseTree syntax, int line, int column)
        {
            for (var i = 0; i < syntax.ChildCount; i++)
            {
                var child = syntax.GetChild(i);
                var foundChild = GetContext(child, line, column);
                if (foundChild != null)
                    return foundChild;

                switch (child)
                {
                    case injectionParser.AdditiveOperandContext additiveOperand:
                        if (additiveOperand.Start.Line > additiveOperand.Stop.Line && additiveOperand.Stop.Line == line)
                        {
                            return new SuggestionContext(SuggestionKind.Namespace | SuggestionKind.Subrutines | SuggestionKind.Variables, null);
                        }
                        break;
                    case injectionParser.OperandContext operand:
                        if (operand.Start.Line == line && operand.Start.Column <= column)
                        {
                            var text = operand.GetText();
                            if (operand.Start.Column + text.Length >= column - 1)
                                return new SuggestionContext(SuggestionKind.Namespace | SuggestionKind.Subrutines | SuggestionKind.Variables, text);
                        }
                        break;
                    case injectionParser.ArgumentContext argument:
                        if (argument.Start.Line == line && argument.Start.Column <= column)
                        {
                            var text = argument.GetText();
                            if (argument.Start.Column + text.Length >= column - 1)
                                return new SuggestionContext(SuggestionKind.Namespace | SuggestionKind.Subrutines | SuggestionKind.Variables, text);
                        }
                        break;
                    case injectionParser.ArgumentListContext argumentList:
                        if (line == argumentList.Start.Line && line == argumentList.Stop.Line)
                        {
                            if (argumentList.Stop.Column < column)
                                return new SuggestionContext(SuggestionKind.Namespace | SuggestionKind.Subrutines | SuggestionKind.Variables, null);
                        }
                        break;
                    case injectionParser.StatementContext statement:
                        if (line == statement.Start.Line && line == statement.Stop.Line)
                            return new SuggestionContext(SuggestionKind.All, statement.GetText());
                        break;
                    case injectionParser.SubrutineContext subrutine:
                        if (line > subrutine.Start.Line && line < subrutine.Stop.Line)
                            return new SuggestionContext(SuggestionKind.All, null);
                        break;
                }
            }

            return null;
        }

        private struct SuggestionContext
        {
            public SuggestionContext(SuggestionKind allowedSuggestions, string prefix)
            {
                AllowedSuggestions = allowedSuggestions;
                Prefix = prefix;
            }

            public SuggestionKind AllowedSuggestions { get; }
            public string Prefix { get; }
            public bool HasNamespace => !string.IsNullOrEmpty(Prefix) && Prefix.Contains('.');

            internal bool IsSameNamespace(string str)
            {
                var dotIndex = str.IndexOf('.');
                var prefixDotIndex = Prefix?.IndexOf('.') ?? -1;

                if (dotIndex < 0 && prefixDotIndex < 0)
                    return true;

                if (prefixDotIndex < 0 && dotIndex > 0)
                    return false;

                if (prefixDotIndex > 0 && dotIndex < 0)
                    return false;

                return str.Substring(0, dotIndex).Equals(Prefix.Substring(0, prefixDotIndex), StringComparison.OrdinalIgnoreCase);
            }

            internal bool StartsWith(string str)
            {
                if (string.IsNullOrEmpty(Prefix))
                    return false;

                return Prefix.StartsWith(str);
            }
        }

        [Flags]
        private enum SuggestionKind
        {
            None = 0,
            Keywords = 1,
            Subrutines = 2,
            Namespace = 4,
            Variables = 8,
            All=15
        }

        private class VariableDefinitionCollector : injectionBaseVisitor<bool>
        {
            private readonly int referenceLine;
            private readonly string prefix;
            private HashSet<string> variableNames = new HashSet<string>();

            public IEnumerable<string> VariableNames => variableNames;

            public VariableDefinitionCollector(int referenceLine, string prefix)
            {
                this.referenceLine = referenceLine;
                this.prefix = prefix;
            }

            public override bool VisitVarDef([NotNull] injectionParser.VarDefContext context)
            {
                if (context.Start.Line < referenceLine)
                    Process(context.SYMBOL() ?? context.assignment().lvalue().SYMBOL());

                return true;
            }

            public override bool VisitDimDef([NotNull] injectionParser.DimDefContext context)
            {
                if (context.Start.Line < referenceLine)
                    Process(context.SYMBOL());

                return true;
            }

            private void Process(ITerminalNode symbol)
            {
                var name = symbol.GetText();
                if (string.IsNullOrEmpty(prefix) || name.StartsWith(prefix))
                    variableNames.Add(name);
            }
        }
    }
}
