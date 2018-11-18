using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Analysis
{
    [TestClass]
    public class MisplacedStatementsTests
    {
        [TestMethod]
        public void Redundand_endif_emits_warning()
        {
            var messages = Parse(@"
sub test()
    if 0 then
        return 2
    endif
    endif

    return 1
end sub");

            messages.AssertWarning(6, "STM001");
        }

        [TestMethod]
        public void While_without_wend_emits_warning()
        {
            var messages = Parse(@"
sub test()
    while 0
end sub");

            messages.AssertWarning(3, "STM002");
        }

        [TestMethod]
        public void While_with_wend_on_other_nesting_level_emits_warning_for_while_and_wend()
        {
            var messages = Parse(@"
sub test()
    while 0
    if 1 then
        wend
    end if
end sub");

            messages.AssertWarning(3, "STM002");
            messages.AssertWarning(5, "STM003");
        }
    }
}
