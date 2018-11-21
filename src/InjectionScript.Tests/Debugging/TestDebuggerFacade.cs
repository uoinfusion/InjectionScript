using FluentAssertions;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InjectionScript.Tests.Debugging
{
    internal sealed class TestDebuggerFacade
    {
        private readonly DebuggerServer debuggerServer = new DebuggerServer();
        private readonly InjectionRuntime runtime;
        private Task subrutineTask;

        public TestDebuggerFacade()
        {
            runtime = new InjectionRuntime(null, debuggerServer);
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
        }

        public void AddBreakpoint(int line) => debuggerServer.AddBreakpoint("test.sc", line);

        public Task CallSubrutineAsync(string name)
        {
            if (subrutineTask != null)
                Assert.Fail("Cannot run more subrutines in parallel.");

            subrutineTask = Task.Run(() => runtime.CallSubrutine(name));

            return subrutineTask;
        }

        internal EvaluationResult EvaluateExpression(string expression)
        {
            var result = debuggerServer.EvaluateExpression(expression);

            result.Result.HasValue.Should().BeTrue("evaluation is expected to succeed");
            result.Messages.Should().BeEmpty("no error messages expected");

            return result;
        }

        public void WaitForBreakpointHit()
            => debuggerServer.BreakpointHitEvent.WaitOne(TimeSpan.FromSeconds(1)).Should().BeTrue("breakpoint is expected to be hit");

        public void Continue() => debuggerServer.Continue();
    }
}
