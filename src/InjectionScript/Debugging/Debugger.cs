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
        private bool traceEnabled = false;
        private readonly RingStringAppender traceBuffer = new RingStringAppender(1024, 1024);

        public Debugger(DebuggerServer server)
        {
            this.server = server;
        }

        public void BeforeStatement(StatementExecutionContext context)
        {
            if (traceEnabled)
            {
                traceBuffer.AppendLine($"Line {context.Line}: {context.GetStatementText()}");
            }

            if (server.TryGetBreakpoint(context.File, context.Line, out var breakpoint))
            {
                server.OnBreak(this, breakpoint);
                currentContext = context;
                continueEvent.WaitOne();
            }
        }

        public void BeforeVariableAssignment(VariableAssignmentContext context)
        {
            if (traceEnabled)
                traceBuffer.AppendLine(context.ToString());
        }

        public void BeforeCall(CallContext context)
        {
            if (traceEnabled)
                traceBuffer.AppendLine(context.ToString());
        }

        public void BeforeReturn(ReturnContext context)
        {
            if (traceEnabled)
                traceBuffer.AppendLine(context.ToString());
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

        internal void DumpTrace(StringBuilder output) => traceBuffer.Dump(output);
        internal void DisableTracing() => traceEnabled = false;
        internal void EnableTracing() => traceEnabled = true;
    }
}
