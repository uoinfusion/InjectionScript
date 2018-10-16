using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
