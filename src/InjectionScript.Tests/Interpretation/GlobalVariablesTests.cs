using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.TestHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class GlobalVariablesTests
    {
        [TestMethod]
        public void Can_set_global_variable_without_local_declaration()
        {
            TestSubrutine(321, "test", @"
var globalVariable

sub test()
    globalVariable = 321
    return globalVariable
end sub
");
        }

        [TestMethod]
        public void Can_get_value_of_global_variable_with_initial_value()
        {
            TestSubrutine(321, "test", @"
var globalVariable = 321

sub test()
    return globalVariable
end sub
");

        }

        [TestMethod]
        public void Can_get_value_of_global_variable_with_initial_value_returned_from_subrutine()
        {
            TestSubrutine(321, "test", @"
sub get_initial_value()
    return 321
end sub

var globalVariable = get_initial_value()

sub test()
    return globalVariable
end sub
");
        }

        [TestMethod]
        public void Value_of_global_variable_is_shared_from_caller_to_callee()
        {
            TestSubrutine(321, "caller", @"
var globalVariable

sub caller()
    globalVariable = 321
    callee()
    
    return callee()
end sub

sub callee()
    return globalVariable
end sub
");
        }

        [TestMethod]
        public void Callee_value_changes_of_global_variable_are_NOT_visible_for_caller()
        {
            TestSubrutine(321, "caller", @"
var globalVariable

sub caller()
    globalVariable = 321
    callee()
    
    return globalVariable
end sub

sub callee()
    globalVariable = 666
end sub
");
        }

        [TestMethod]
        public void Aliasing_local_variable_value_is_NOT_visible_to_callee()
        {
            TestSubrutine(321, "caller", @"
var aliasedGlobalVariable = 321

sub caller()
    var aliasedGlobalVariable = 666

    return callee()
end sub

sub callee()
    return aliasedGlobalVariable
end sub
");
        }
    }
}
