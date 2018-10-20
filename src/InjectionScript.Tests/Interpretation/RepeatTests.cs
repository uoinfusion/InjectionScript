using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;


namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class RepeatTests
    {
        [TestMethod]
        public void Trivial_repeat()
        {
            TestSubrutine(10, @"var i = 0
repeat
    i = i + 1
until i < 10

return i");
        }
    }
}
