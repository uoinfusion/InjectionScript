using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            TestSubrutine(1, @"var i = 0
if 1 then
    i = i + 1
else
    i = i + 333
endif

return i");
        }

        [TestMethod]
        public void Nested_true_conditions_with_else()
        {
            TestSubrutine(1, @"var i = 0
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
else
    i = i + 333
endif

return i");
        }

        [TestMethod]
        public void Nested_false_conditions_with_else()
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

        [TestMethod]
        public void Parser_emits_warning_for_redundand_endif()
        {
            var messages = Parse(@"if 0 then
    return 2
endif
endif

return 1");

            messages.Any(x => x.Severity == MessageSeverity.Warning && x.Line == 4);
        }
    }
}
