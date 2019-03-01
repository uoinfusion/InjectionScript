using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using InjectionScript.Runtime.Contexts;
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
        private readonly Func<CancellationToken?> retrieveCancellationToken;
        private readonly AutoResetEvent continueEvent = new AutoResetEvent(false);
        private StatementExecutionContext currentContext;
        private bool traceEnabled = false;
        private readonly RingStringAppender traceBuffer = new RingStringAppender(1024, 1024);
        private bool breakNextStatement = false;

        public Debugger(DebuggerServer server, Func<CancellationToken?> retrieveCancellationToken)
        {
            this.server = server;
            this.retrieveCancellationToken = retrieveCancellationToken;
        }

        public void BeforeStatement(StatementExecutionContext context)
        {
            if (traceEnabled)
            {
                traceBuffer.AppendLine($"Line {context.Line}: {context.GetStatementText()}");
            }

            if (server.TryGetBreakpoint(context.File, context.Line, out var breakpoint))
            {
                server.OnBreak(this, new BreakpointDebuggerBreak(breakpoint));
                currentContext = context;
                WaitForContinue();
            }
            else if (breakNextStatement)
            {
                breakNextStatement = false;
                server.OnBreak(this, new StepDebuggerBreak(new SourceCodeLocation(context.File, context.Line)));
                WaitForContinue();
            }
        }

        private void WaitForContinue()
        {
            var cancellationToken = retrieveCancellationToken?.Invoke();
            if (cancellationToken.HasValue)
            {
                WaitHandle.WaitAny(new WaitHandle[] { continueEvent, cancellationToken.Value.WaitHandle });
            }
            else
                continueEvent.WaitOne();

            server.OnDebuggerResumed();
        }

        public void BeforeVariableAssignment(VariableAssignmentContext context)
        {
            if (traceEnabled)
                traceBuffer.AppendLine(context.ToString());
        }

        public void AfterCall(AfterCallContext context)
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
            breakNextStatement = false;
            continueEvent.Set();
        }

        public void Step()
        {
            breakNextStatement = true;
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
        public void ExecutionFailed(Exception ex) => server.OnDebuggerResumed();
    }
}
