using FluentAssertions;
using InjectionScript.Runtime;
using InjectionScript.Runtime.ObjectTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class ObjectCallTests
    {
        private class SomeInjObj : InjectionObject
        {
            public SomeInjObj() : base("SomeInjObj")
            {
                Register(new NativeSubrutineDefinition("MySubrutine", (Func<InjectionValue>)MySubrutine));
            }

            public InjectionValue MySubrutine() => new InjectionValue("some text");
        }

        [TestMethod]
        public void Calls_subrutine_on_object()
        {
            TestSubrutine("some text", @"
var obj = CreateObj()
return obj.MySubrutine()", new[]
            {
                new NativeSubrutineDefinition("CreateObj", () => new InjectionValue(new SomeInjObj()))
            });
        }
    }
}

