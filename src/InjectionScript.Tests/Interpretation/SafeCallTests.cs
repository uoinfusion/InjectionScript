using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class SafeCallTests
    {
        [TestMethod]
        public void Safe_call_calls_custom_subrutine()
        {
            TestSubrutine(333, "sub1", @"sub sub1()
    return safe call sub2()
end sub

sub sub2()
    return 333
end sub");
        }

        [TestMethod]
        public void Safe_call_calls_native_subrutine_in_expression()
        {
            TestSubrutine(333, "sub1", @"sub sub1()
    var x = '123'
    return safe call val(x) + 210
end sub");
        }
    }
}
