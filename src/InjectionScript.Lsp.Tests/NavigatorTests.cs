using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Lsp.Tests
{
    [TestClass]
    public class NavigatorTests
    {
        [TestMethod]
        public void Call_statement_to_subrutine_definition()
        {
            string file = @"
sub test1()
    test1()
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 3, 7);

            definitionLocation.ShouldBeInRange(2, 5, 10);
        }

        [TestMethod]
        public void Call_statement_to_subrutine_definition_when_cursor_at_name_end()
        {
            string file = @"
sub test1()
    test1()
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 3, 10);

            definitionLocation.ShouldBeInRange(2, 5, 10);
        }

        [TestMethod]
        public void Call_statement_to_subrutine_definition_regarding_parameter_count()
        {
            string file = @"
sub test1()
    test1(123)
end sub

sub test1(param1)
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 3, 10);

            definitionLocation.ShouldBeInRange(6, 5, 10);
        }

        [TestMethod]
        public void Call_operand_to_subrutine_definition()
        {
            string file = @"
sub test1()
    test1(test1(1 + test1()))
end sub

sub test1(param1)
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 3, 24);

            definitionLocation.ShouldBeInRange(2, 5, 10);
        }

        [TestMethod]
        public void Variable_in_lvalue_to_definition()
        {
            string file = @"
sub test1()
    var variable_name
    
    variable_name = variable_name + 1
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 5, 11);

            definitionLocation.ShouldBeInRange(3, 9, 22);
        }

        [TestMethod]
        public void Variable_in_operand_to_definition()
        {
            string file = @"
sub test1()
    var variable_name
    
    variable_name = variable_name + 1
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 5, 24);

            definitionLocation.ShouldBeInRange(3, 9, 22);
        }

        [TestMethod]
        public void Dim_in_lvalue_to_definition()
        {
            string file = @"
sub test1()
    dim dim_name[10]
    
    dim_name[1] = dim_name[1] + 1
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 5, 9);

            definitionLocation.ShouldBeInRange(3, 9, 21);
        }

        [TestMethod]
        public void Dim_in_operand_to_definition()
        {
            string file = @"
sub test1()
    dim dim_name[10]
    
    dim_name[1] = dim_name[1] + 1
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 5, 22);

            definitionLocation.ShouldBeInRange(3, 9, 21);
        }
        [TestMethod]
        public void Goto_label_to_definition()
        {
            string file = @"
sub test1()
label1:
    goto label1
end sub
";
            var definitionLocation = GetDefinitionLocation(file, 4, 13);

            definitionLocation.ShouldBeInRange(3, 1, 7);
        }

        private Location GetDefinitionLocation(string fileContent, int line, int column)
        {
            var navigator = new Navigator(null);
            return navigator.GetDefinition(fileContent, line, column);
        }
    }

    public static class RangeExtensions
    {
        public static void ShouldBeInRange(this Location location, int line, int startColumn, int endColumn)
        {
            if (location == null)
                Assert.Fail("Expecting non-null location, but the location is null");

            var range = location.Range;

            if (range.Start.Line != line)
                Assert.Fail($"Range is expected to start at line {line} but it starts at {range.Start.Line}");
            if (range.End.Line != line)
                Assert.Fail($"Range is expected to end at line {line} but it ends at {range.End.Line}");

            if (range.Start.Character != startColumn)
                Assert.Fail($"Range is expected to start at column {startColumn} but it starts at {range.Start.Character}");
            if (range.End.Character != endColumn)
                Assert.Fail($"Range is expected to end at column {endColumn} but it ends at {range.End.Character}");
        }
    }
}
