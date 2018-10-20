using InjectionScript.Interpretation;
using InjectionScript.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace InjectionScript.Tests.Interpretation
{
    public static class InterpretationHelpers
    {
        public static InjectionValue EvalExpression(string expression)
        {
            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            var expressionSyntax = parser.ParseExpression(expression);
            var runtime = new Runtime();

            return runtime.Interpreter.VisitExpression(expressionSyntax);
        }

        public static void TestExpression(string expression, int expectedValue)
        {
            var result = EvalExpression(expression);

            Assert.AreEqual(InjectionValueKind.Number, result.Kind, expression);
            Assert.AreEqual(expectedValue, result.Number, expression);
        }

        public static void TestSubrutine(int expected, string codeBlock)
        {
            string subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new Runtime();
            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(subrutine));

            var actual = runtime.CallSubrutine("test");

            Assert.AreEqual(InjectionValueKind.Number, actual.Kind, codeBlock);
            Assert.AreEqual(expected, actual.Number, codeBlock);
        }

        public static void TestSubrutine(int expected, string subrutineName, string file)
        {
            var runtime = new Runtime();
            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(file));

            var actual = runtime.CallSubrutine(subrutineName);

            Assert.AreEqual(InjectionValueKind.Number, actual.Kind, file);
            Assert.AreEqual(expected, actual.Number, file);
        }
    }
}
