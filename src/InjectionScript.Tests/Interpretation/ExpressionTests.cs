using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static InjectionScript.Tests.Interpretation.InterpretationHelpers;

namespace InjectionScript.Tests.Interpretation
{
    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void Unary_operators()
        {
            TestExpression("-1", -1);
            TestExpression("--1", 1);
            TestExpression("not 5", 0);
            TestExpression("not 0", 1);
            TestExpression("not -1", 0);
        }

        [TestMethod]
        public void Binary_operators()
        {
            TestExpression("1+1", 2);
            TestExpression("1+1+1", 3);
            TestExpression("1-1", 0);
            TestExpression("2*3", 6);
            TestExpression("2*3*4", 24);
            TestExpression("6/3", 2);
            TestExpression("30/3/2", 5);

            TestExpression("1 && 1", 1);
            TestExpression("0 && 1", 0);
            TestExpression("1 and 1", 1);

            TestExpression("1 || 0", 1);
            TestExpression("0 || 0", 0);
            TestExpression("0 or 1", 1);

            TestExpression("1 == 0", 0);
            TestExpression("5 == 5", 1);
            TestExpression("1 <> 0", 1);
            TestExpression("5 <> 5", 0);
        }

        [TestMethod]
        public void Operator_precedence()
        {
            TestExpression("1+2*3", 7);
            TestExpression("1 + (2 * 3)", 7);
            TestExpression("(1 + 2) * 3", 9);
            TestExpression("5 - 1 && 1", 1);
            TestExpression("5 - 1 || 1", 1);
            TestExpression("5 - (1 && 1)", 4);

            TestExpression("5 - (1 || 1)", 4);
            TestExpression("6 / 2 || 1", 1);
            TestExpression("6 / 2 || 1", 1);
            TestExpression("1 + 0 == 0 + 1", 1);
            TestExpression("1 + (0 == 0) + 1", 3);
            TestExpression("5 * 1 == 5", 1);
            TestExpression("5 * (1 == 5)", 0);
        }
    }
}
