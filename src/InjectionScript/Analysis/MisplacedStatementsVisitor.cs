using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System.Collections.Generic;

namespace InjectionScript.Analysis
{
    internal sealed class MisplacedStatementsVisitor : injectionBaseVisitor<bool>
    {
        private readonly List<Message> messages;

        public MisplacedStatementsVisitor(List<Message> messages)
        {
            this.messages = messages;
        }

        public override bool VisitMissplacedEndif([NotNull] injectionParser.MissplacedEndifContext context)
        {
            messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                MessageSeverity.Warning, MessageCodes.MisplacedEndIf,
                "Cannot find any related 'if' to this 'end if'. Please, remove it, or pair it with an 'if'."));

            return true;
        }

        public override bool VisitIncompleteWhile([NotNull] injectionParser.IncompleteWhileContext context)
        {
            messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                MessageSeverity.Warning, MessageCodes.IncompleteWhile,
                "Cannot find any related 'wend' to this 'while'. Please, close this 'while' with a 'wend' on the same nesting level."));

            return true;
        }

        public override bool VisitWend([NotNull] injectionParser.WendContext context)
        {
            messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                MessageSeverity.Warning, MessageCodes.MisplacedWend,
                "Cannot find any related 'while' to this 'wend'. Please, remove it, or pair it with an 'while'."));

            return true;
        }
    }

}
