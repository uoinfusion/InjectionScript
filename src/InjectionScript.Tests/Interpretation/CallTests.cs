using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

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

        [TestMethod]
        public void Call_with_arguments_and_return_value()
        {
            TestSubrutine(5, "sub1", @"sub sub1()
    return sub2(3, 2)
end sub

sub sub2(a, b)
    return a+b
end sub");
        }

        [TestMethod]
        public void Multiple_subrutines_with_same_name_and_different_parameters()
        {
            TestSubrutine(9, "sub1", @"sub sub1()
    return sub2(2) + sub2(2, 3)
end sub

sub sub2(a, b)
    return a + b
end sub

sub sub2(a)
    return 2 * a
end sub
");
        }

        [TestMethod]
        public void Subrutine_name_can_contain_dot()
        {
            TestSubrutine(9, "s.u.b.1...", @"sub s.u.b.1...()
    return 9
end sub
");
        }

        [TestMethod]
        public void Can_call_empty_subrutine()
        {
            TestSubrutine(9, "sub1", @"
sub sub1()
    empty()
    return 9
end sub

sub empty()
end sub
");
        }
    }
}
