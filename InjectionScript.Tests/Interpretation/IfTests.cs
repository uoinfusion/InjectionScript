using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class IfTests
    {
        [TestMethod]
        public void If_trivial_true_condition()
        {
            TestSubrutine(2, @"if 1 then
    return 2
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
        public void If_trivial_true_condition_with_else()
        {
            TestSubrutine(2, @"if 1 then
    return 2
else
    return 3
endif

return 1");
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
