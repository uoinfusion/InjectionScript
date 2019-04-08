using FluentAssertions;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Tests.Debugging
{
    internal sealed class TestDebuggerFacade
    {
        private readonly DebuggerServer debuggerServer;
        private readonly ITracer tracer;
        private readonly InjectionRuntime runtime;
        private readonly AutoResetEvent breakHitEvent = new AutoResetEvent(false);
        private Task subrutineTask;

        public CancellationTokenSource ScriptCancellation { get; private set; }

        public DebuggerBreak LastBreak { get; private set; }

        public TestDebuggerFacade()
            : this(new RealTimeSource())
        {
        }

        public TestDebuggerFacade(ITimeSource timeSource)
        {
            ScriptCancellation = new CancellationTokenSource();
            debuggerServer = new DebuggerServer(() => ScriptCancellation.Token);

            runtime = new InjectionRuntime(null, debuggerServer, timeSource, () => ScriptCancellation.Token);
            tracer = debuggerServer;
            debuggerServer.DebuggerBreakHit += HandleDebuggerBreakHit;
        }

        private void HandleDebuggerBreakHit(object sender, DebuggerBreak e)
        {
            LastBreak = e;
            breakHitEvent.Set();
        }

        public void Load(string sourceCode)
        {
            var messages = runtime.Load(sourceCode, "test.sc");
            if (messages.Any())
            {
                Assert.Fail("Script loading error found: " + messages.ToString());
            }
        }

        internal void AssertSubrutineFinished()
        {
            if (subrutineTask == null)
                Assert.Fail("No subrutine started.");

            subrutineTask.Wait(TimeSpan.FromSeconds(1)).Should().BeTrue("subrutine is expected to finish");
            subrutineTask = null;
        }

        public void AddBreakpoint(int line) => debuggerServer.AddBreakpoint("test.sc", line);
        public void Step() => debuggerServer.Step();

        public Task CallSubrutineAsync(string name)
        {
            if (subrutineTask != null)
                Assert.Fail("Cannot run more subrutines in parallel.");

            ScriptCancellation = new CancellationTokenSource();

            subrutineTask = Task.Run(() => runtime.CallSubrutine(name));

            return subrutineTask;
        }

        public void CallSubrutine(string name) => runtime.CallSubrutine(name);

        internal EvaluationResult EvaluateExpression(string expression)
        {
            ScriptCancellation = new CancellationTokenSource();
            var result = debuggerServer.EvaluateExpression(expression);

            result.Result.HasValue.Should().BeTrue("evaluation is expected to succeed");
            result.Messages.Should().BeEmpty("no error messages expected");

            return result;
        }

        public void WaitForBreakHit()
            => breakHitEvent.WaitOne(TimeSpan.FromSeconds(10)).Should().BeTrue("breakpoint is expected to be hit in 10 seconds");

        public void Continue() => debuggerServer.Continue();

        public void EnabledTracing() => tracer.Enable();
        public void DisableTracing() => tracer.Disable();
        public string DumpTrace() => tracer.Dump();
    }
}
