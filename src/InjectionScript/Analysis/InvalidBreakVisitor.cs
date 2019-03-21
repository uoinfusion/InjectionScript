using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Analysis
{
    public class InvalidBreakVisitor : injectionBaseVisitor<bool>
    {
        private readonly List<Message> messages;
        private readonly Stack<MessageSeverity?> breakScopeValidity = new Stack<MessageSeverity?>();

        public InvalidBreakVisitor(List<Message> messages)
        {
            this.messages = messages;
        }

        private MessageSeverity? ContextSeverity
        {
            get
            {
                if (!breakScopeValidity.Any())
                    return MessageSeverity.Error;

                return breakScopeValidity.Peek();
            }
        }

        private void ResetContext()
        {
            breakScopeValidity.Clear();
        }

        private void StartContext(MessageSeverity? severity)
        {
            breakScopeValidity.Push(severity);
        }

        private void StartValidContext()
        {
            StartContext(null);
        }

        private void EndContext()
        {
            if (breakScopeValidity.Any())
                breakScopeValidity.Pop();
        }

        public override bool VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            ResetContext();
            StartValidContext();

            base.VisitSubrutine(context);

            EndContext();

            return true;
        }

        public override bool VisitCodeBlock([NotNull] injectionParser.CodeBlockContext context)
        {
            foreach (var statement in context.statement())
            {
                if (statement.@for() != null)
                    StartContext(MessageSeverity.Error);
                else if (statement.next() != null)
                    EndContext();
                else if (statement.@while() != null)
                    StartValidContext();
                else if (statement.wend() != null)
                    EndContext();
                else
                    Visit(statement);
            }

            return true;
        }

        public override bool VisitIf([NotNull] injectionParser.IfContext context)
        {
            StartContext(MessageSeverity.Warning);

            var result = base.VisitIf(context);

            EndContext();

            return result;
        }

        public override bool VisitBreak([NotNull] injectionParser.BreakContext context)
        {
            if (!ContextSeverity.HasValue)
                return true;

            switch (ContextSeverity.Value)
            {
                case MessageSeverity.Error:
                    messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                        ContextSeverity.Value, MessageCodes.InvalidBreak,
                        $"'break' keyword is not valid at this line. It can be directly inside subrutine, while and repeat cycles."));
                    break;
                case MessageSeverity.Warning:
                    messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                        ContextSeverity.Value, MessageCodes.InvalidBreak,
                        $"'break' keyword is accepted by Injection in this context but it does nothing."));
                    break;
            }

            return false;
        }
    }
}
