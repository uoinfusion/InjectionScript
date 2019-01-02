using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class ShortcutVariableTests
    {
        [TestMethod]
        public void Shortcut_variables_default_values()
        {
            TestSubrutine("self", "return self");
            TestSubrutine("lastcorpse", "return lastcorpse");
            TestSubrutine("finditem", "return finditem");
            TestSubrutine("backpack", "return backpack");
        }

        [TestMethod]
        public void Can_change_value_of_shortcut_variables()
        {
            TestSubrutine("asdf", @"self = 'asdf'
return self");
            TestSubrutine("asdf", @"lastcorpse = 'asdf'
return lastcorpse");
            TestSubrutine("asdf", @"finditem = 'asdf'
return finditem");
        }
    }
}
