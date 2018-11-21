using FluentAssertions;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace InjectionScript.Tests.Debugging
{
    [TestClass]
    public class ExpressionEvaluationTests
    {
        [TestMethod]
        public void Can_evaluate()
        {
            var debuggerServer = new DebuggerServer();
            var runtime = new InjectionRuntime(null, debuggerServer);

            runtime.Load(@"
sub sub1()
    var x = 33
    x = x + 1
end sub", "test.sc");

            debuggerServer.AddBreakpoint("test.sc", 4);
            Task.Run(() =>
            {
                runtime.CallSubrutine("sub1");
            });

            debuggerServer.BreakpointHitEvent.WaitOne();
            var result = debuggerServer.EvaluateExpression("x");

            result.Result.Value.Should().Be(33);
        }

        [TestMethod]
        public void Can_evaluate_continue_and_evaluate_again()
        {
            var debuggerServer = new DebuggerServer();
            var runtime = new InjectionRuntime(null, debuggerServer);

            runtime.Load(@"
sub sub1()
    var x = 33
    x = x + 1
    x = x + 1
end sub", "test.sc");

            debuggerServer.AddBreakpoint("test.sc", 4);
            debuggerServer.AddBreakpoint("test.sc", 5);

            Task.Run(() =>
            {
                runtime.CallSubrutine("sub1");
            });

            debuggerServer.BreakpointHitEvent.WaitOne();

            var result = debuggerServer.EvaluateExpression("x");
            result.Result.Value.Should().Be(33);

            debuggerServer.Continue();
            debuggerServer.BreakpointHitEvent.WaitOne();

            result = debuggerServer.EvaluateExpression("x");
            result.Result.Value.Should().Be(34);
        }
    }
}
