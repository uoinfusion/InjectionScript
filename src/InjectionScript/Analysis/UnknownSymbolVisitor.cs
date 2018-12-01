using Antlr4.Runtime.Misc;
using InjectionScript.Runtime;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Analysis
{
    internal sealed class UnknownSymbolVisitor : injectionBaseVisitor<bool>
    {
        private readonly List<Message> messages;
        private readonly Metadata metadata;
        private readonly HashSet<string> varNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public UnknownSymbolVisitor(List<Message> messages, Metadata metadata)
        {
            this.messages = messages;
            this.metadata = metadata;
        }

        public override bool VisitCall([NotNull] injectionParser.CallContext context)
        {
            var name = context.SYMBOL().GetText();

            var argumentCount = context.argumentList().arguments()?.argument()?.Count() ?? 0;

            if (!metadata.NativeSubrutineExists(name, argumentCount)
                && !metadata.TryGetSubrutine(name, argumentCount, out var customSubrutine))
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                    MessageSeverity.Warning, MessageCodes.UndefinedSubrutine,
                    $"Subrutine {name} with {argumentCount} arguments not found."));
            }

            return true;
        }

        public override bool VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            varNames.Clear();

            return base.VisitSubrutine(context);
        }

        public override bool VisitParameterName([NotNull] injectionParser.ParameterNameContext context)
        {
            varNames.Add(context.SYMBOL().GetText());

            return true;
        }

        public override bool VisitVarDef([NotNull] injectionParser.VarDefContext context)
        {
            var name = context.SYMBOL()?.GetText() ?? context.assignment()?.lvalue()?.SYMBOL()?.GetText();
            varNames.Add(name);

            return true;
        }

        public override bool VisitDimDef([NotNull] injectionParser.DimDefContext context)
        {
            var name = context.SYMBOL()?.GetText();
            varNames.Add(name);

            return base.VisitDimDef(context);
        }

        public override bool VisitAssignment([NotNull] injectionParser.AssignmentContext context)
        {
            var name = context.lvalue()?.SYMBOL()?.GetText();
            if (!string.IsNullOrEmpty(name) && !varNames.Contains(name))
            {
                messages.Add(new Message(context.lvalue().Start.Line, context.lvalue().Start.Column, context.lvalue().Stop.Line, context.lvalue().Stop.Column,
                    MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                    $"Variable not found '{name}'."));
            }
            else
            {
                name = context.lvalue()?.indexedSymbol()?.SYMBOL()?.GetText();
                if (!string.IsNullOrEmpty(name) && !varNames.Contains(name) && !metadata.TryGetIntrinsicVariable(name, out _))
                {
                    messages.Add(new Message(context.lvalue().Start.Line, context.lvalue().Start.Column, context.lvalue().Stop.Line, context.lvalue().Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                        $"Variable not found '{name}'."));
                }
            }

            return base.VisitAssignment(context);
        }

        public override bool VisitOperand([NotNull] injectionParser.OperandContext context)
        {
            var varName = context.SYMBOL()?.GetText();
            if (!string.IsNullOrEmpty(varName) && !varNames.Contains(varName) && 
                !metadata.TryGetIntrinsicVariable(varName, out _))
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                    MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                    $"Variable not found '{varName}'."));
            }
            else
            {
                var dimName = context.indexedSymbol()?.SYMBOL()?.GetText();
                if (!string.IsNullOrEmpty(dimName) && !varNames.Contains(dimName))
                {
                    messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                    $"Variable not found '{dimName}'."));
                }
            }

            return base.VisitOperand(context);
        }
    }
}
