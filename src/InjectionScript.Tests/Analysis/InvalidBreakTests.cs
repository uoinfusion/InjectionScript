using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Analysis
{
    [TestClass]
    public class InvalidBreakTests
    {
        [TestMethod]
        public void Break_inside_for_cycle_emits_error()
        {
            var messages = Parse(@"
sub test()
    for var i = 0 to 1
        break
    next
end sub");

            messages.AssertError(4, MessageCodes.InvalidBreak);
        }

        [TestMethod]
        public void Break_inside_while_nested_in_for_is_ok()
        {
            var messages = Parse(@"
sub test()
    for var i = 0 to 1
        while 1
            break
        wend
    next
end sub");

            messages.AssertNoError();
        }

        [TestMethod]
        public void Break_inside_if_emits_warning()
        {
            var messages = Parse(@"
sub test()
    if 1 then
        break
    endif
end sub");

            messages.AssertWarning(4, MessageCodes.InvalidBreak);
        }

        [TestMethod]
        public void Break_inside_while_nested_in_if_no_warning()
        {
            var messages = Parse(@"
sub test()
    if 1 then
        while 1
            break
        wend
    endif
end sub");

            messages.AssertNoWarning(5, MessageCodes.InvalidBreak);
        }
    }
}
