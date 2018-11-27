using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Analysis
{
    [TestClass]
    public class UnknownSymbolTests
    {
        [TestMethod]
        public void Warning_when_a_subrutine_not_defined()
        {
            var messages = Parse(@"
sub test()
    unknown()
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedSubrutine);
        }

        [TestMethod]
        public void No_warning_when_a_subrutine_defined()
        {
            var messages = Parse(@"
sub test()
    test()
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedSubrutine);
        }

        [TestMethod]
        public void No_warning_when_a_subrutine_defined_with_different_casing()
        {
            var messages = Parse(@"
sub test()
    TEST()
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedSubrutine);
        }

        [TestMethod]
        public void Warning_when_assigning_to_undefined_variable()
        {
            var messages = Parse(@"
sub test()
    x = 123
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_assigning_to_defined_variable()
        {
            var messages = Parse(@"
sub test()
    var x
    x = 123
end sub");

            messages.AssertNoWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_assigning_to_variable_defined_with_different_casing()
        {
            var messages = Parse(@"
sub test()
    var VAR1
    var1 = 123
end sub");

            messages.AssertNoWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_for_var_def_assignment()
        {
            var messages = Parse(@"
sub test()
    var x = 123
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void Warning_for_undefined_var_in_var_def_assignment()
        {
            var messages = Parse(@"
sub test()
    var x = z
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_for_parameters()
        {
            var messages = Parse(@"
sub test(z)
    var x = z + 2
    x = z + 1
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedVariable);
            messages.AssertNoWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void Warning_when_assigning_to_undefined_dim()
        {
            var messages = Parse(@"
sub test()
    x[4] = 123
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_assigning_to_defined_dim()
        {
            var messages = Parse(@"
sub test()
    dim x[10]
    x[4] = 123
end sub");

            messages.AssertNoWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void Warning_when_reading_from_undefined_variable()
        {
            var messages = Parse(@"
sub test()
    var x
    x = y
end sub");

            messages.AssertWarning(4, MessageCodes.UndefinedVariable);

        }

        [TestMethod]
        public void Warning_when_reading_from_undefined_dim()
        {
            var messages = Parse(@"
sub test()
    var x
    x = y[5]
end sub");

            messages.AssertWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_reading_from_defined_dim()
        {
            var messages = Parse(@"
sub test()
    var x
    dim y[10]
    x = y[5]
end sub");

            messages.AssertNoWarning(5, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_reading_from_intrinsic_variable()
        {
            var messages = Parse(@"
sub test()
    var x = true
end sub");

            messages.AssertNoWarning(5, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_when_reading_from_dim_defined_with_different_casing()
        {
            var messages = Parse(@"
sub test()
    var x
    dim var1[10]
    x = VAR1[5]
end sub");

            messages.AssertNoWarning(5, MessageCodes.UndefinedVariable);
        }
    }
}
