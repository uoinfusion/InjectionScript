using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class ForTests
    {
        [TestMethod]
        public void For_variable_is_equal_to_range_when_loop_finishes()
        {
            TestSubrutine(10, @"var i
for i = 0 to 10
next
return i");
        }

        [TestMethod]
        public void Number_of_cycles_is_range_minus_start_plus_one()
        {
            TestSubrutine(11, @"var i
var x = 0
for i = 0 to 10
    x = x + 1
next
return x");
        }

        [TestMethod]
        public void Constant_ranges_empty_body_starting_non_zero()
        {
            TestSubrutine(6, @"var i
var num = 0
for i = 5 to 10
    num = num + 1
next
return num");
        }

        [TestMethod]
        public void Constant_ranges_nested_next()
        {
            TestSubrutine(5, @"var i
for i = 0 to 10
    if i < 5 then
        next
    end if
return i");
        }

        [TestMethod]
        public void Jumping_to_false_if()
        {
            TestSubrutine(10, @"var i, x = 1
if x then
    x = 0
    for i = 0 to 10
end if
next
    
return i");
        }

        [TestMethod]
        public void Nested_loops()
        {
            TestSubrutine(60, @"var i, j, k
var num = 0
for i = 0 to 2
    for j = 0 to 3 
        for k = 0 to 4
            num = num + 1
        next
    next
next
return num");
        }

        [TestMethod]
        public void Out_of_range_before_start()
        {
            TestSubrutine(2, @"
var i, x = 1
for i = 1 to 1
    x = x + 1
next

return x");
        }
    }
}
