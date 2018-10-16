using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class CallTests
    {
        [TestMethod]
        public void Call_with_return_value()
        {
            TestSubrutine(333, "sub1", @"sub sub1()
    return sub2()
end sub

sub sub2()
    return 333
end sub");
        }
    }
}
