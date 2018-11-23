using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Tests.Debugging
{
    [TestClass]
    public class TracingTests
    {
        [TestMethod]
        public void Records_statement()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    var x = 33
end sub");

            testDebugger.EnabledTracing();
            testDebugger.CallSubrutine("sub1");

            var trace = testDebugger.DumpTrace();

            Assert.IsTrue(trace.Contains("varx=33"));
        }

        [TestMethod]
        public void Records_arguments()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    sub2('param1 value', 314)
end sub

sub sub2(param1, param2)
    var x = 1
end sub
");

            testDebugger.EnabledTracing();
            testDebugger.CallSubrutine("sub1");

            var trace = testDebugger.DumpTrace();

            Assert.IsTrue(trace.Contains("param1 value"));
            Assert.IsTrue(trace.Contains("314"));
        }

        [TestMethod]
        public void Records_return_value()
        {
            var testDebugger = new TestDebuggerFacade();

            testDebugger.Load(@"
sub sub1()
    sub2()
end sub

sub sub2()
    return 'return value'
end sub
");

            testDebugger.EnabledTracing();
            testDebugger.CallSubrutine("sub1");

            var trace = testDebugger.DumpTrace();

            Assert.IsTrue(trace.Contains("return value"));
        }
    } 
}
