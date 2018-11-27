using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class IfTests
    {
        [TestMethod]
        public void If_trivial_true_condition()
        {
            TestSubrutine(2, @"
var x = 0
if 1 then
    x = x + 1
endif

x = x + 1

return x");
        }

        [TestMethod]
        public void If_trivial_true_condition_with_empty_then()
        {
            TestSubrutine(1, @"if 1 then
endif

return 1");
        }

        [TestMethod]
        public void If_trivial_false_condition()
        {
            TestSubrutine(1, @"if 0 then
    return 2
endif

return 1");
        }

        [TestMethod]
        public void If_trivial_false_condition_with_empty_then()
        {
            TestSubrutine(1, @"if 0 then
endif

return 1");
        }

        [TestMethod]
        public void If_trivial_true_condition_with_else()
        {
            TestSubrutine(3, @"var i = 0
if 1 then
    i = i + 1
else
    i = i + 333
endif

i = i + 2

return i");
        }

        [TestMethod]
        public void If_trivial_true_condition_with_empty_else()
        {
            TestSubrutine(0, @"var i = 0
if 1 then
else
endif

return i");
        }

        [TestMethod]
        public void If_trivial_false_condition_with_empty_else()
        {
            TestSubrutine(0, @"var i = 0
if 0 then
else
endif

return i");
        }

        [TestMethod]
        public void Nested_true_conditions_with_else()
        {
            TestSubrutine(3, @"var i = 0
if 1 then
    if 1 then
        if 1 then
            i = i + 1
        else
            i = i + 335
        end if
    else
        i = i + 334
    end if
    i = i + 1
else
    i = i + 333
endif

i = i + 1

return i");
        }

        [TestMethod]
        public void Nested_false_conditions()
        {
            TestSubrutine(2, @"var i = 0
if 0 then
    if 0 then
        i = i + 334
    else
        if 0 then
            i = i + 335
        else
            i = i + 347
        end if
        i = i + 358
    end if
    i = i + 369
else
    i = i + 1
endif
i = i + 1

return i");
        }

        [TestMethod]
        public void Nested_false_condition_within_else()
        {
            TestSubrutine(1, @"var i = 0
if 0 then
    i = i + 333
else
    if 0 then
        i = i + 334
    else
        if 0 then
            i = i + 335
        else
            i = i + 1
        end if
    end if
endif

return i");
        }

        [TestMethod]
        public void If_trivial_false_condition_with_else()
        {
            TestSubrutine(3, @"if 0 then
    return 2
else
    return 3
endif

return 1");
        }
    }
}
