using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using InjectionScript.Interpretation;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Analysis
{
    public sealed class CallWalker 
    {
        private class Visitor : injectionBaseVisitor<bool>
        {
            private readonly Action<injectionParser.CallContext> callback;

            public Visitor(Action<injectionParser.CallContext> callback)
            {
                this.callback = callback;
            }

            public override bool VisitCall([NotNull] injectionParser.CallContext context)
            {
                callback(context);
                return true;
            }
        }

        public void Walk(IParseTree tree)
        {
            var visitor = new Visitor(VisitCall);
            visitor.Visit(tree);
        }

        public Action<injectionParser.CallContext> VisitCall { get; set; }
    }
}
