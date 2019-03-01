using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Tests.Debugging
{
    [TestClass]
    public class CancellationTests
    {
        [TestMethod]
        public void Stops_debugging_when_script_cancelled()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
end sub");

            testDebugger.AddBreakpoint(3);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();
            testDebugger.ScriptCancellation.Cancel();
            testDebugger.AssertSubrutineFinished();
        }

        [TestMethod]
        public void Breakpoint_works_after_cancellation()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
end sub");

            testDebugger.AddBreakpoint(3);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();
            testDebugger.ScriptCancellation.Cancel();
            testDebugger.AssertSubrutineFinished();

            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();
            testDebugger.Continue();
            testDebugger.AssertSubrutineFinished();
        }
    }
}
