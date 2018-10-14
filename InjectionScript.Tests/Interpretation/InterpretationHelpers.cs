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
            var expressionSyntax = Parser.ParseExpression(expression);
            var runtime = new Runtime();

            return runtime.Interpreter.VisitExpression(expressionSyntax);
        }

        public static void TestExpression(string expression, int expectedValue)
        {
            var result = EvalExpression(expression);

            Assert.AreEqual(InjectionValueKind.Number, result.Kind, expression);
            Assert.AreEqual(expectedValue, result.Number, expression);
        }
    }
}
