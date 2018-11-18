using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class NativeCallTests
    {
        [TestMethod]
        public void Subrutine_names_are_case_insensitive()
        {
            TestSubrutine("some text", @"return UO.MyNaTiVe()", new[]
            {
                new NativeSubrutineDefinition("UO.mynative", (Func<string>)(() => "some text"))
            });
        }

        [TestMethod]
        public void Can_overload_with_different_parameter_count()
        {
            TestSubrutine(5, @"return UO.sub1() + UO.sub1(3)", new[]
            {
                new NativeSubrutineDefinition("UO.sub1", (Func<int>)(() => 2)),
                new NativeSubrutineDefinition("UO.sub1", (Func<int, int>)((a) => a))
            });

        }

        [TestMethod]
        public void Can_overload_with_different_parameter_type()
        {
            TestSubrutine(5, @"return UO.sub1(""0x123"") + UO.sub1(0x123)", new[]
            {
                new NativeSubrutineDefinition("UO.sub1", (Func<int, int>)((a) => 2)),
                new NativeSubrutineDefinition("UO.sub1", (Func<string, int>)((a) => 3))
            });
        }

        [TestMethod]
        public void Intrinsic_variable_can_call_native_subrutine()
        {
            TestSubrutine(123, @"return UO.IntrinsicVar", intrinsicVariables: new[]
            {
                new NativeSubrutineDefinition("UO.IntrinsicVar", (Func<int>)(() => 123)),
            });
        }
    }
}
