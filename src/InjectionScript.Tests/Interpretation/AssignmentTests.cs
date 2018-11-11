using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class AssignmentTests
    {
        [TestMethod]
        public void Numeric_assignment()
        {
            TestSubrutine(3, @"var x
x = 1+2
return x");
        }

        [TestMethod]
        public void Numeric_declaration_assignment()
        {
            TestSubrutine(3, @"var x = 1+2
return x");
        }

        [TestMethod]
        public void Multiple_numeric_declaration_assignments()
        {
            TestSubrutine(3, @"var x=3,y=4
return x");
        }

        [TestMethod]
        public void Multiple_numeric_declaration()
        {
            TestSubrutine(6, @"var x,y
y = 3
x = y * 2
return x");
        }

        [TestMethod]
        public void Variable_name_can_contain_dot()
        {
            TestSubrutine(123, @"var x.y = 123
return x.y");

        }
    }
}
