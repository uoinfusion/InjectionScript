using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class BreakTests
    {
        [TestMethod]
        public void Break_terminates_repeat()
        {
            TestSubrutine(0, @"var i = 0
repeat
    break
    i = i + 1
until i >= 10

return i");
        }

        [TestMethod]
        public void Break_terminates_inner_nested_repeat_doesnt_terminate_outer_repeat()
        {
            TestSubrutine(100, @"var i = 0
var j = 0

repeat
    repeat
        break
        i = i + 1
    until i >= 10
    j = j + 100
until j >= 1
return i + j");
        }

        [TestMethod]
        public void Break_inside_sub_code_block_breaks_repeat()
        {
            TestSubrutine(0, @"var i = 0
repeat
    if 1 then
        break
    end if
    i = i + 1
until i >= 1
return i");
        }

        [TestMethod]
        public void Break_terminates_while()
        {
            TestSubrutine(0, @"var i = 0
while i < 1
    break
    i = i + 1
wend
return i");
        }

        [TestMethod]
        public void Break_terminates_inner_nested_while_doesnt_terminate_outer_while()
        {
            TestSubrutine(100, @"var i = 0
var j = 0
while j < 1
    while i < 1
        break
        i = i + 1
    wend
    j = j + 100
wend
return i +  j");
        }

        [TestMethod]
        public void Break_inside_sub_code_block_breaks_while()
        {
            TestSubrutine(0, @"var i = 0
while i < 1
    if 1 then
        break
    end if
    i = i + 1
wend
return i");
        }

        [TestMethod]
        public void Break_inside_subrutine_does_nothing()
        {
            TestSubrutine(0, @"break
return 0");
        }

        [TestMethod]
        public void Break_inside_if_does_nothing()
        {
            TestSubrutine(1, @"var i = 0
if 1 then
    break
    i = i + 1
endif
return i");
        }
    }
}
