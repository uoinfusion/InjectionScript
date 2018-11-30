using FluentAssertions;
using InjectionScript.Parsing;
using InjectionScript.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InjectionScript.Lsp.Tests
{
    [TestClass]
    public class CompleterTests
    {
        [TestMethod]
        public void Statement_call_with_UO_prefix_after_dot()
        {
            string file = @"
sub test1()
    UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 8);

            completions.ShouldContainLabel("ignore", "attack");
        }

        [TestMethod]
        public void Statement_call_with_UO_prefix_Then_no_statement_keywords()
        {
            string file = @"
sub test1()
    UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 8);

            completions.ShouldNotContainLabel("if", "repeat");
        }

        [TestMethod]
        public void Statement_call_with_UO_prefix_without_dot_Then_no_subrutines_in_UO_namespace()
        {
            string file = @"
sub test1()
    UO
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 7);

            completions.ShouldNotContainLabel("ignore", "attack");
        }

        [TestMethod]
        public void Empty_statement_line_Then_suggests_flow_statement_keywords()
        {
            string file = @"
sub test1()
    
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 5);

            completions.ShouldContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");
        }

        [TestMethod]
        public void Empty_statement_line_Then_suggests_subrutines()
        {
            string file = @"
sub test1()
    
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 5);

            completions.ShouldContainLabel("test1");
        }

        [TestMethod]
        public void Empty_statement_line_Then_suggests_native_subrutines_without_namespace()
        {
            string file = @"
sub test1()
    
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 5);

            completions.ShouldContainLabel("wait", "str", "val");
        }

        [TestMethod]
        public void Empty_statement_line_Then_suggests_no_native_subrutines_in_UO_namespace()
        {
            string file = @"
sub test1()
    
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 5);

            completions.ShouldNotContainLabel("attack", "ignore");
        }

        [TestMethod]
        public void Statement_keyword_prefix_Then_suggests_flow_statement_keywords()
        {
            string file = @"
sub test1()
    el    
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 7);

            completions.ShouldContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");

        }

        [TestMethod]
        public void Subrutine_prefix_Then_suggests_subrutine_names()
        {
            string file = @"
sub test1()
    te   
end sub

sub test_subrutine()
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 7);

            completions.ShouldContainLabel("test1", "test_subrutine");

        }

        [TestMethod]
        public void UO_prefix_Then_suggests_UO_namespace()
        {
            string file = @"
sub test1()
    U  
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 6);

            completions.ShouldContainLabel("UO");
        }

        [TestMethod]
        public void Global_native_subrutine_prefix_Then_suggests_global_native_subrutine()
        {
            string file = @"
sub test1()
    wa
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 7);

            completions.ShouldContainLabel("wait");
        }

        [TestMethod]
        public void Argument_statement_call_with_UO_prefix_after_dot()
        {
            string file = @"
sub test1()
    UO.Print(UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 17);

            completions.ShouldContainLabel("ignore", "attack");
        }

        [TestMethod]
        public void Argument_statement_call_with_UO_prefix_Then_no_statement_keywords()
        {
            string file = @"
sub test1()
    UO.Print(UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 17);

            completions.ShouldNotContainLabel("if", "repeat");
        }

        [TestMethod]
        public void Argument_statement_call_with_UO_prefix_without_dot_Then_no_subrutines_in_UO_namespace()
        {
            string file = @"
sub test1()
    UO.Print(UO
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 16);

            completions.ShouldNotContainLabel("ignore", "attack");
        }

        [TestMethod]
        public void Argument_list_empty_and_closed_Then_suggests_everything_except_flow_statement_keywords()
        {
            string file = @"
sub test()
   UO.Print()
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 13);

            completions.ShouldContainLabel("test");
            completions.ShouldContainLabel("UO");
            completions.ShouldContainLabel("str");
            completions.ShouldNotContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");
        }

        [TestMethod]
        public void Argument_empty_Then_suggests_no_flow_statement_keywords()
        {
            string file = @"
sub test1()
    UO.Print(
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 14);

            completions.ShouldNotContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");
        }

        [TestMethod]
        public void Argument_empty_Then_suggests_subrutines()
        {
            string file = @"
sub test1()
    UO.Print(
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 14);

            completions.ShouldContainLabel("test1");
        }

        [TestMethod]
        public void Argument_empty_Then_suggests_native_subrutines_without_namespace()
        {
            string file = @"
sub test1()
    UO.Print(
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 14);

            completions.ShouldContainLabel("wait", "str", "val");
        }

        [TestMethod]
        public void Argument_empty_Then_suggests_no_native_subrutines_in_UO_namespace()
        {
            string file = @"
sub test1()
    UO.Print(
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 14);

            completions.ShouldNotContainLabel("attack", "ignore");
        }

        [TestMethod]
        public void Argument_statement_keyword_prefix_Then_suggests_flow_statement_keywords()
        {
            string file = @"
sub test1()
    UO.Print(el
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 16);

            completions.ShouldNotContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");

        }

        [TestMethod]
        public void Argument_subrutine_prefix_Then_suggests_subrutine_names()
        {
            string file = @"
sub test1()
    UO.Print(te
end sub

sub test_subrutine()
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 16);

            completions.ShouldContainLabel("test1", "test_subrutine");

        }

        [TestMethod]
        public void Argument_UO_prefix_Then_suggests_UO_namespace()
        {
            string file = @"
sub test1()
    UO.Print(U
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 15);

            completions.ShouldContainLabel("UO");
        }

        [TestMethod]
        public void Argument_UO_namespace_Then_suggests_UO_subrutines()
        {
            string file = @"
sub test1()
    UO.Print(UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 17);

            completions.ShouldContainLabel("attack");
        }

        [TestMethod]
        public void Argument_global_native_subrutine_prefix_Then_suggests_global_native_subrutine()
        {
            string file = @"
sub test1()
    UO.Print(s
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 15);

            completions.ShouldContainLabel("str");
        }

        [TestMethod]
        public void Second_argument_empty_Then_suggests_no_flow_statement_keywords()
        {
            string file = @"
sub test1()
    UO.Print('param1',
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 23);

            completions.ShouldNotContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");
        }

        [TestMethod]
        public void Second_argument_empty_Then_suggests_subrutines()
        {
            string file = @"
sub test1()
    UO.Print('param1',
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 23);

            completions.ShouldContainLabel("test1");
        }

        [TestMethod]
        public void Second_paramter_empty_Then_suggests_native_subrutines_without_namespace()
        {
            string file = @"
sub test1()
    UO.Print('param1',
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 23);

            completions.ShouldContainLabel("wait", "str", "val");
        }

        [TestMethod]
        public void Second_argument_empty_Then_suggests_no_native_subrutines_in_UO_namespace()
        {
            string file = @"
sub test1()
    UO.Print('param1',
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 23);

            completions.ShouldNotContainLabel("attack", "ignore");
        }

        [TestMethod]
        public void Second_argument_statement_keyword_prefix_Then_suggests_flow_statement_keywords()
        {
            string file = @"
sub test1()
    UO.Print('param1',el
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 25);

            completions.ShouldNotContainLabel("if", "else", "end if", "repeat", "until", "while", "wend",
                "for", "next", "return", "goto", "var", "dim");

        }

        [TestMethod]
        public void Second_argument_subrutine_prefix_Then_suggests_subrutine_names()
        {
            string file = @"
sub test1()
    UO.Print('param1',te
end sub

sub test_subrutine()
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 25);

            completions.ShouldContainLabel("test1", "test_subrutine");

        }

        [TestMethod]
        public void Second_argument_UO_prefix_Then_suggests_UO_namespace()
        {
            string file = @"
sub test1()
    UO.Print('param1',U
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 24);

            completions.ShouldContainLabel("UO");
        }

        [TestMethod]
        public void Second_argument_global_native_subrutine_prefix_Then_suggests_global_native_subrutine()
        {
            string file = @"
sub test1()
    UO.Print('param1',s
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 24);

            completions.ShouldContainLabel("str");
        }

        [TestMethod]
        public void First_argument_UO_prefix_Second_parameter_global_native_subrutine_prefix_Then_suggests_global_native_subrutine_for_second_parameter()
        {
            string file = @"
sub test1()
    UO.Print(UO.,s
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 19);

            completions.ShouldContainLabel("str");
            completions.ShouldNotContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void First_argument_global_native_subrutine_prefix_Second_parameter_UO_prefix_Then_suggests_UO_subrutines_for_second_parameter()
        {
            string file = @"
sub test1()
    UO.Print(s,UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 19);

            completions.ShouldNotContainLabel("str");
            completions.ShouldContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void First_argument_UO_prefix_and_Second_parameter_global_native_subrutine_prefix_Then_suggests_UO_subrutines_for_first_parameter()
        {
            string file = @"
sub test1()
    UO.Print(UO.,s
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 17);

            completions.ShouldNotContainLabel("str");
            completions.ShouldContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void First_argument_global_native_subrutine_prefix_Second_parameter_UO_prefix_Then_suggests_global_native_subrutine_for_first_parameter()
        {
            string file = @"
sub test1()
    UO.Print(s,UO.
end sub
";
            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 15);

            completions.ShouldContainLabel("str");
            completions.ShouldNotContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void Argument_operand_with_UO_namespace_Then_suggests_UO_namespace_subrutines()
        {
            string file = @"
sub test1()
    UO.Print('x' + UO.)
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 23);

            completions.ShouldNotContainLabel("str");
            completions.ShouldContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void Argument_empty_operand_Then_suggests_subrutines_without_namespace()
        {
            string file = @"
sub test1()
    UO.Print('x' + )
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 20);

            completions.ShouldContainLabel("str");
            completions.ShouldNotContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void Argument_empty_operand_Then_suggests_variables()
        {
            string file = @"
sub test1()
    var myVariableBefore
    dim myDimBefore = 123

    UO.Print('x' + )

    var myVariableAfter
    dim myDimAfter = 123
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 6, 20);

            completions.ShouldContainLabel("myVariableBefore", "myDimBefore");
            completions.ShouldNotContainLabel("myVariableAfter", "myDimAfter");
        }

        [TestMethod]
        public void Assignment_operand_with_UO_namespace_Then_suggests_UO_namespace_subrutines()
        {
            string file = @"
sub test1()
    var x = 123 + UO.
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 22);

            completions.ShouldNotContainLabel("str");
            completions.ShouldContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void Assignment_empty_operand_Then_suggests_subrutines_without_namespace()
        {
            string file = @"
sub test1()
    var x = 123 + 
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 3, 19);

            completions.ShouldContainLabel("str");
            completions.ShouldNotContainLabel("attack");
            completions.ShouldNotContainLabel("if");
        }

        [TestMethod]
        public void Assignment_empty_operand_Then_suggests_variables_defined_before_assignment()
        {
            string file = @"
sub test1()
    var myVariableBefore
    dim myDimBefore

    var x = 123 + my

    var myVariableAfter
    dim myDimAfter
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 6, 21);

            completions.ShouldContainLabel("myVariableBefore", "myDimBefore");
            completions.ShouldNotContainLabel("myVariableAfter", "myDimAfter");
        }


        [TestMethod]
        public void Outside_subrutine_Then_suggests_sub()
        {
            string file = @"

sub test1()
end sub";

            var completer = new Completer();
            var completions = completer.GetCompletions(file, 2, 1);

            completions.ShouldNotContainLabel("str");
            completions.ShouldNotContainLabel("attack");
            completions.ShouldNotContainLabel("if");
            completions.ShouldContainLabel("sub", "end sub");
        }
    }
}
