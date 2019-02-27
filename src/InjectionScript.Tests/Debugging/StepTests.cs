using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Tests.Debugging
{
    [TestClass]
    public class StepTests
    {
        [TestMethod]
        public void Can_step_to_next_statement()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
    x = x + 1
end sub");

            testDebugger.AddBreakpoint(3);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();

            testDebugger.Step();
            testDebugger.WaitForBreakHit();
            testDebugger.LastBreak?.Location.Line.Should().Be(4);
        }

        [TestMethod]
        public void Doesnt_stop_at_empty_line()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33

    x = x + 1
end sub");

            testDebugger.AddBreakpoint(3);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakHit();

            testDebugger.Step();
            testDebugger.WaitForBreakHit();
            testDebugger.LastBreak?.Location.Line.Should().Be(5);
        }
    }
}
