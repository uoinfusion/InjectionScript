using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class DimTests
    {
        [TestMethod]
        public void Dim_with_constant_limit()
        {
            TestSubrutine(123, @"dim x[10]
x[5] = 123
return x[5]
");
        }

        [TestMethod]
        public void Dim_name_can_contain_dot()
        {
            TestSubrutine(123, @"dim x.y...[10]
x.y...[5] = 123
return x.y...[5]
");
        }

        [TestMethod]
        public void Dim_multiple_on_one_line_with_constant_limit()
        {
            TestSubrutine(3, @"   dim x[10], y[10]
   x[5] = 1
   y[6] = 2

    return x[5] + y[6]
");
        }

        [TestMethod]
        public void Dim_zero_index_access()
        {
            TestSubrutine(123, @"   dim x[10]
   
   x[0] = 123
   return x[0]
");
        }

        [TestMethod]
        public void Dim_limit_index_access()
        {
            TestSubrutine(123, @"   dim x[10]
   
   x[10] = 123

   return x[10]
");
        }

        [TestMethod]
        public void Dim_converts_to_scalar_after_assignment_without_index()
        {
            TestSubrutine(2, @"   dim x[10]

   x[5] = 1   
   x = 2

   return x
");
        }

        [TestMethod]
        [ExpectedException(typeof(ScriptFailedException))]
        public void Exception_when_reading_from_uninitialized_index()
        {
            TestSubrutine(1, @"
dim x[10]
var y

y = x[5]
");
        }

        [TestMethod]
        public void Dim_cannot_access_dim_when_converted_scalar()
        {
            try
            {
                TestSubrutine(1, @"   dim x[10]

   x[5] = 1   
   x = 2

   return x[5]
");
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail("Exception expected.");
        }
    }
}
