using InjectionScript.Parsing;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Debugging
{
    public class DebuggerServer : IDebuggerServer
    {
        private Debugger activeDebugger;
        private readonly object activeDebuggerLock = new object();
        private readonly object breakpointLock = new object();
        private readonly List<Breakpoint> breakpoints = new List<Breakpoint>();
        private readonly Parser parser = new Parser();

        public ManualResetEvent BreakpointHitEvent { get; } = new ManualResetEvent(false);
        public event EventHandler<Breakpoint> BreakpointHit;

        public void Continue()
        {
            BreakpointHitEvent.Reset();

            lock (activeDebuggerLock)
            {
                if (activeDebugger != null)
                {
                    activeDebugger.Continue();
                    activeDebugger = null;
                }
            }
        }

        public EvaluationResult EvaluateExpression(string expressionText)
        {
            lock (activeDebuggerLock)
            {
                if (activeDebugger != null)
                {
                    var expression = parser.ParseExpression(expressionText, out var messages);
                    return new EvaluationResult(messages, activeDebugger.EvaluateExpression(expression));
                }
                else
                    throw new NotImplementedException();
            }
        }

        internal void OnBreak(Debugger debugger, Breakpoint breakpoint)
        {
            lock (activeDebuggerLock)
            {
                if (activeDebugger != null)
                    throw new NotImplementedException("Multiple active debuggers.");
                activeDebugger = debugger;
                BreakpointHitEvent.Set();
            }

            BreakpointHit?.Invoke(this, breakpoint);
        }

        public void AddBreakpoint(string fileName, int line)
        {
            lock (breakpointLock)
            {
                breakpoints.Add(new Breakpoint(fileName, line));
            }
        }

        public bool TryGetBreakpoint(string fileName, int line, out Breakpoint breakpoint)
        {
            lock (breakpointLock)
            {
                breakpoint = breakpoints.FirstOrDefault(x => x.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase) && line == x.Line);
                return breakpoint != null;
            }
        }

        IDebugger IDebuggerServer.Create() => new Debugger(this);
    }
}
