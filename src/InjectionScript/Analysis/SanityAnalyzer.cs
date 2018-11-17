using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Analysis
{
    public class SanityAnalyzer
    {
        private class Visitor : injectionBaseVisitor<bool>
        {
            public List<Message> Messages { get; } = new List<Message>();

            public override bool VisitEndif([NotNull] injectionParser.EndifContext context)
            {
                Messages.Add(new Message(context.Start.Line, context.Start.Column, "Cannot find any related 'if' to this 'end if'. Please, remove it, or pair it with an 'if'.", MessageSeverity.Warning));

                return true;
            }
        }

        public MessageCollection Analyze(injectionParser.FileContext fileContext)
        {
            var visitor = new Visitor();
            visitor.Visit(fileContext);

            return new MessageCollection(visitor.Messages);
        }
    }
}
