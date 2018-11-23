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

        [TestMethod]
        public void Can_pass_dim_as_a_parameter()
        {
            TestSubrutine(123, "sub1", @"
sub sub1()
    dim x[10]

    sub2(x)

    return x[5]
end sub

sub sub2(y)
    y[5] = 123
end sub
");
        }

        [TestMethod]
        public void Can_return_dim_from_subrutine()
        {
            TestSubrutine(123, "sub1", @"
sub sub1()
    var x
    x = sub2(x)
    return x[5]
end sub

sub sub2(y)
    dim y[10]
    y[5] = 123
    return y
end sub
");

            Assert.Inconclusive("Has to be tested on injection.");
        }

        [TestMethod]
        public void Dim_can_contain_different_type_on_different_indexes()
        {
            TestSubrutine("test123", @"
    dim x[10]
    x[1] = 123
    x[2] = 'test'

    return x[2] + str(x[1]);
");

            Assert.Inconclusive("Check it on injection.");
        }
    }
}
