using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Analysis
{
    [TestClass]
    public class InvalidSymbolTests
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
        public void Warning_when_a_subrutine_not_defined_in_vardef()
        {
            var messages = Parse(@"
sub test()
    var x = unknown()
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedSubrutine);
        }

        [TestMethod]
        public void Warning_when_a_subrutine_not_defined_in_assignment()
        {
            var messages = Parse(@"
sub test()
    var x
    x = unknown()
end sub");

            messages.AssertWarning(4, MessageCodes.UndefinedSubrutine);
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

            messages.AssertWarning(3, MessageCodes.UndefinedVariable);
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
        public void No_warning_for_global_variables()
        {
            var messages = Parse(@"
var x = 1;
sub test()
    x = x + 1
end sub");

            messages.AssertNoWarning(4, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_for_variable_declared_in_for_cycle()
        {
            var messages = Parse(@"
sub test()
    for var i = 0 to 10
    next
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedVariable);
        }

        [TestMethod]
        public void No_warning_for_shortcut_variable()
        {
            var messages = Parse(@"
sub test()
    UO.Print(backpack)
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedVariable);
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
        public void Warning_when_reading_from_undefined_variable_as_argument()
        {
            var messages = Parse(@"
sub test(param1)
    test(undefined)
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedVariable);
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
        public void Warning_when_reading_from_undefined_dim_as_argument()
        {
            var messages = Parse(@"
sub test(param1)
    test(y[5])
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedVariable);
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

        [TestMethod]
        public void Warning_for_undefined_label()
        {
            var messages = Parse(@"
sub test()
    goto undefinedlabel
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedLabel);
        }

        [TestMethod]
        public void Warning_for_mutliple_gotos_to_same_undefined_label()
        {
            var messages = Parse(@"
sub test()
    goto undefinedlabel
    goto undefinedlabel
end sub");

            messages.AssertWarning(3, MessageCodes.UndefinedLabel);
            messages.AssertWarning(4, MessageCodes.UndefinedLabel);
        }

        [TestMethod]
        public void No_warning_for_forward_defined_label()
        {
            var messages = Parse(@"
sub test()
    goto undefinedlabel
undefinedlabel:
    return
end sub");

            messages.AssertNoWarning(3, MessageCodes.UndefinedLabel);
        }

        [TestMethod]
        public void No_warning_for_backward_defined_label()
        {
            var messages = Parse(@"
sub test()
undefinedlabel:
    goto undefinedlabel
    return
end sub");

            messages.AssertNoWarning(4, MessageCodes.UndefinedLabel);
        }

        [TestMethod]
        public void Warning_for_label_name_followed_by_invalid_chars()
        {
            var messages = Parse(@"
sub test()
validLabel:
    goto validLabel:+-23545:
    return
end sub");

            messages.AssertWarning(4, MessageCodes.InvalidLabelName);
        }
    }
}
