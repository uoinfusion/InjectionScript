using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System.Collections.Generic;

namespace InjectionScript.Analysis
{
    public class SanityAnalyzer
    {
        private class MisplacedStatementsVisitor : injectionBaseVisitor<bool>
        {
            private readonly List<Message> messages;

            public MisplacedStatementsVisitor(List<Message> messages)
            {
                this.messages = messages;
            }

            public override bool VisitMissplacedEndif([NotNull] injectionParser.MissplacedEndifContext context)
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, MessageSeverity.Warning, "STM001",
                    "Cannot find any related 'if' to this 'end if'. Please, remove it, or pair it with an 'if'."));

                return true;
            }

            public override bool VisitIncompleteWhile([NotNull] injectionParser.IncompleteWhileContext context)
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, MessageSeverity.Warning, "STM002",
                    "Cannot find any related 'wend' to this 'while'. Please, close this 'while' with a 'wend' on the same nesting level."));

                return true;
            }

            public override bool VisitWend([NotNull] injectionParser.WendContext context)
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, MessageSeverity.Warning, "STM003",
                    "Cannot find any related 'while' to this 'wend'. Please, remove it, or pair it with an 'while'."));

                return true;
            }
        }

        public MessageCollection Analyze(injectionParser.FileContext fileContext)
        {
            var messages = new List<Message>();

            var extraneousEndifVisitor = new MisplacedStatementsVisitor(messages);
            extraneousEndifVisitor.Visit(fileContext);

            return new MessageCollection(messages);
        }
    }
}
