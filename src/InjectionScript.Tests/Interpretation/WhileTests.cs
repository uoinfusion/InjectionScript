using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;


namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class WhileTests
    {
        [TestMethod]
        public void Trivial_while()
        {
            TestSubrutine(10, @"var i = 0
while i < 10
    i = i + 1
wend
return i");
        }

        [TestMethod]
        public void While_with_always_false_condition()
        {
            TestSubrutine(0, @"var i = 0
while 0
    i = i + 1
wend
return i");
        }

        [TestMethod]
        public void Nested_while_inner_always_false()
        {
            TestSubrutine(0, @"
var i = 4
var j = 0
while i <> 0
    while 0
        j = j + 1
    wend
    i = i - 1
wend
return i");
        }
    }
}
