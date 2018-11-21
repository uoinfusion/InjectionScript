using FluentAssertions;
using InjectionScript.Debugging;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Tests.Debugging
{
    [TestClass]
    public class BreakpointTests
    {
        [TestMethod]
        public void Can_break_at_end_sub()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
end sub");

            testDebugger.AddBreakpoint(4);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakpointHit();

            testDebugger.Continue();
            testDebugger.AssertSubrutineFinished();
        }

        [TestMethod]
        public void Can_break_at_statement()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
end sub");

            testDebugger.AddBreakpoint(3);
            testDebugger.CallSubrutineAsync("sub1");
            testDebugger.WaitForBreakpointHit();

            testDebugger.Continue();
            testDebugger.AssertSubrutineFinished();
        }
    }
}
