using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    public class Debugger : IDebugger
    {
        private readonly DebuggerServer server;
        private readonly AutoResetEvent continueEvent = new AutoResetEvent(false);
        private StatementExecutionContext currentContext;

        public Debugger(DebuggerServer server)
        {
            this.server = server;
        }

        public void BeforeStatement(StatementExecutionContext context)
        {
            if (server.TryGetBreakpoint(context.File, context.Line, out var breakpoint))
            {
                server.OnBreak(this);
                currentContext = context;
                continueEvent.WaitOne();
            }
        }

        public void Continue()
        {
            currentContext = null;
            continueEvent.Set();
        }

        public InjectionValue? EvaluateExpression(injectionParser.ExpressionContext expression)
        {
            if (currentContext == null)
                return null;

            return currentContext.Eval(expression);
        }
    }
}
