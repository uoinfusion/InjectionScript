using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

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

            messages.AssertWarning(6, MessageCodes.MisplacedEndIf);
        }

        [TestMethod]
        public void While_without_wend_emits_warning()
        {
            var messages = Parse(@"
sub test()
    while 0
end sub");

            messages.AssertWarning(3, MessageCodes.IncompleteWhile);
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

            messages.AssertWarning(3, MessageCodes.IncompleteWhile);
            messages.AssertWarning(5, MessageCodes.MisplacedWend);
        }

        [TestMethod]
        public void Orphaned_else_emits_warning()
        {
            var messages = Parse(@"
sub test()
    else
        return 1
    endif
end sub");

            messages.AssertWarning(3, MessageCodes.OrphanedElse);
        }

        [TestMethod]
        public void Valid_else_without_warning()
        {
            var messages = Parse(@"
sub test()
    if 0 then
        return 1
    else
        return 2
    endif
end sub");

            messages.AssertNoWarning(4, MessageCodes.OrphanedElse);
        }
    }
}
