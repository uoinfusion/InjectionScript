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
//        [TestMethod]
//        public void Can_evaluate()
//        {
//            var testDebugger = new TestDebuggerFacade();

//            testDebugger.Load(@"
//sub sub1()
//    var x = 33
//    x = x + 1
//end sub");

//            testDebugger.AddBreakpoint(4);
//            testDebugger.CallSubrutineAsync("sub1");
//            testDebugger.WaitForBreakHit();

//            var result = testDebugger.EvaluateExpression("x");

//            result.Result.Value.Should().Be(33);
//        }

        [TestMethod]
        public void Can_evaluate_continue_and_evaluate_again()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
    x = x + 1
    x = x + 1
end sub");

            testDebugger.AddBreakpoint(4);
            testDebugger.AddBreakpoint(5);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();

            var result = testDebugger.EvaluateExpression("x");
            result.Result.Value.Should().Be(33);

            testDebugger.Continue();
            testDebugger.WaitForBreakHit();

            result = testDebugger.EvaluateExpression("x");
            result.Result.Value.Should().Be(34);

            testDebugger.Continue();

            testDebugger.AssertSubrutineFinished();
        }
    }
}
