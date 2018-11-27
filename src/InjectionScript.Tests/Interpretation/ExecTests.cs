using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class ExecSubrutineTests
    {
        [TestMethod]
        public void Can_execute_uo_subrutine_without_arguments()
        {
            bool executed = false;

            TestSubrutine(@"uo.exec(""mynative"")", natives: new[]
            {
                new NativeSubrutineDefinition("UO.mynative", (Action)(() => executed = true))
            });

            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void Can_execute_uo_subrutine_with_one_string_argument()
        {
            string passedValue = string.Empty;

            TestSubrutine(@"uo.exec(""mynative argumentvalue"")", natives: new[]
            {
                new NativeSubrutineDefinition("UO.mynative", (Action<string>)((str) => passedValue = str))
            });

            Assert.AreEqual("argumentvalue", passedValue);
        }

        [TestMethod]
        public void Can_execute_uo_subrutine_with_one_quoted_string_argument()
        {
            string passedValue = string.Empty;

            TestSubrutine(@"uo.exec(""mynative 'argumentvalue'"")", natives: new[]
            {
                new NativeSubrutineDefinition("UO.mynative", (Action<string>)((str) => passedValue = str))
            });

            Assert.AreEqual("argumentvalue", passedValue);
        }

        [TestMethod]
        public void Can_execute_custom_subrutine_when_uo_subrutine_not_available()
        {
            TestSubrutine(123, "test", @"
sub test()
    UO.Exec('test2')

    return val(UO.GetGlobal('myglobal'))
end sub

sub test2()
    UO.SetGlobal('myglobal', 123)
end sub
");
        }
    }
}
