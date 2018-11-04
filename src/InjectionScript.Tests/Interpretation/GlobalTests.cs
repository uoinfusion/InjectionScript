using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class GlobalTests
    {
        [TestMethod]
        public void Can_set_and_read_global()
        {
            TestSubrutine("some text", @"UO.SetGlobal(""globname"", ""some text"")
return UO.GetGlobal(""globname"")");
        }
    }
}
