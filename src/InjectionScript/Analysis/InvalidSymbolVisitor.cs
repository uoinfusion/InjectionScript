using Antlr4.Runtime.Misc;
using InjectionScript.Runtime;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace InjectionScript.Analysis
{
    internal sealed class InvalidSymbolVisitor : injectionBaseVisitor<bool>
    {
        private readonly List<Message> messages;
        private readonly Metadata metadata;
        private readonly HashSet<string> varNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> declaredLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<injectionParser.GotoContext> referencedLabels 
            = new List<injectionParser.GotoContext>();

        public InvalidSymbolVisitor(List<Message> messages, Metadata metadata)
        {
            this.messages = messages;
            this.metadata = metadata;
        }

        public override bool VisitCall([NotNull] injectionParser.CallContext context)
        {
            var name = context.SYMBOL().GetText();
            if (!name.Contains('.'))
            {
                var argumentCount = context.argumentList().arguments()?.argument()?.Count() ?? 0;

                if (!metadata.NativeSubrutineExists(name, argumentCount)
                    && !metadata.TryGetSubrutine(name, argumentCount, out var customSubrutine))
                {
                    messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedSubrutine,
                        $"Subrutine {name} with {argumentCount} arguments not found."));
                }
            }

            return base.VisitCall(context);
        }

        public override bool VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            varNames.Clear();
            declaredLabels.Clear();
            referencedLabels.Clear();

            var result = base.VisitSubrutine(context);

            foreach (var gotoContext in referencedLabels)
            {
                var labelName = gotoContext.SYMBOL().GetText();
                if (!declaredLabels.Contains(labelName))
                {
                    messages.Add(new Message(gotoContext.Start.Line, gotoContext.Start.Column, gotoContext.Stop.Line, gotoContext.Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedLabel,
                        $"Label not found '{labelName}'."));
                }
            }

            return result;
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

            return base.VisitVarDef(context);
        }

        public override bool VisitForVarDef([NotNull] injectionParser.ForVarDefContext context)
        {
            var name = context.assignment()?.lvalue()?.SYMBOL()?.GetText();
            if (!string.IsNullOrEmpty(name))
                varNames.Add(name);

            return base.VisitForVarDef(context);
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
            if (!string.IsNullOrEmpty(name))
            {
                if (!varNames.Contains(name) && !metadata.GlobalVariableExists(name)
                    && !metadata.ShortcutVariableExists(name))
                {
                    messages.Add(new Message(context.lvalue().Start.Line, context.lvalue().Start.Column, context.lvalue().Stop.Line, context.lvalue().Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                        $"Variable not found '{name}'."));
                }
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

        public override bool VisitGoto([NotNull] injectionParser.GotoContext context)
        {
            referencedLabels.Add(context);

            if (!string.IsNullOrEmpty(context.invalid?.Text))
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                    MessageSeverity.Warning, MessageCodes.InvalidLabelName,
                    $"Label name '{context.SYMBOL().GetText()}' is followed by invalid characters '{context.invalid?.Text}'. Remove them, they are ignored by Injection."));
            }

            return base.VisitGoto(context);
        }

        public override bool VisitLabel([NotNull] injectionParser.LabelContext context)
        {
            declaredLabels.Add(context.SYMBOL().GetText());

            return base.VisitLabel(context);
        }

        public override bool VisitOperand([NotNull] injectionParser.OperandContext context)
        {
            var varName = context.SYMBOL()?.GetText();
            if (!string.IsNullOrEmpty(varName))
            {
                if (!varNames.Contains(varName) && !metadata.TryGetIntrinsicVariable(varName, out _) 
                    && !metadata.ShortcutVariableExists(varName) && !metadata.GlobalVariableExists(varName))
                {
                    messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                        MessageSeverity.Warning, MessageCodes.UndefinedVariable,
                        $"Variable not found '{varName}'."));
                }
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
