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
        public void Constant_ranges_empty_body()
        {
            TestSubrutine(10, @"var i
for i = 0 to 10
next
return i");
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
    }
}
