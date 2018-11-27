using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Analysis
{
    [TestClass]
    public class RedefinitionTests
    {
        [TestMethod]
        public void Warnings_for_all_methods_with_same_name_and_no_arguments()
        {
            var messages = Parse(@"
sub test()
end sub

sub test()
end sub

sub test()
end sub");

            messages.AssertWarning(2, MessageCodes.SubrutineRedefined);
            messages.AssertWarning(5, MessageCodes.SubrutineRedefinition);
            messages.AssertWarning(8, MessageCodes.SubrutineRedefinition);
        }

        [TestMethod]
        public void No_warnings_for_all_methods_with_same_name_and_same_arguments_count()
        {
            var messages = Parse(@"
sub test(a, b, c)
end sub

sub test(b, c, d)
end sub

sub test(e, f, g)
end sub");

            messages.AssertWarning(2, MessageCodes.SubrutineRedefined);
            messages.AssertWarning(5, MessageCodes.SubrutineRedefinition);
            messages.AssertWarning(8, MessageCodes.SubrutineRedefinition);
        }

        [TestMethod]
        public void No_warnings_for_all_methods_with_same_name_and_different_arguments_count()
        {
            var messages = Parse(@"
sub test(a)
end sub

sub test(b, c)
end sub

sub test(e, f, g)
end sub");

            messages.AssertNoWarning(2, MessageCodes.SubrutineRedefined);
            messages.AssertNoWarning(5, MessageCodes.SubrutineRedefinition);
            messages.AssertNoWarning(8, MessageCodes.SubrutineRedefinition);
        }
    }
}
