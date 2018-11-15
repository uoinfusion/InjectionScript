using InjectionScript.Interpretation;
using InjectionScript.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Assert.AreEqual(new InjectionValue(expectedValue), result, expression);
        }

        public static void TestExpression(string expression, double expectedValue)
        {
            var result = EvalExpression(expression);

            Assert.AreEqual(new InjectionValue(expectedValue), result, expression);
        }

        public static void TestExpression(string expression, string expectedValue)
        {
            var result = EvalExpression(expression);

            Assert.AreEqual(new InjectionValue(expectedValue), result, expression);
        }

        public static void TestSubrutine(string codeBlock, NativeSubrutineDefinition[] natives = null, NativeSubrutineDefinition[] intrinsicVariables = null)
        {
            var subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new Runtime();
            if (natives != null)
                runtime.Metadata.Add(natives);
            if (intrinsicVariables != null)
                runtime.Metadata.AddIntrinsicVariables(intrinsicVariables);

            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(subrutine));

            runtime.CallSubrutine("test");
        }

        public static void TestSubrutine(int expected, string codeBlock, NativeSubrutineDefinition[] natives = null, NativeSubrutineDefinition[] intrinsicVariables = null)
        {
            var subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new Runtime();
            if (natives != null)
                runtime.Metadata.Add(natives);
            if (intrinsicVariables != null)
                runtime.Metadata.AddIntrinsicVariables(intrinsicVariables);

            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(subrutine));

            var actual = runtime.CallSubrutine("test");

            Assert.AreEqual(InjectionValueKind.Integer, actual.Kind, codeBlock);
            Assert.AreEqual(expected, actual.Integer, codeBlock);
        }

        public static void TestSubrutine(string expected, string codeBlock, NativeSubrutineDefinition[] natives = null)
        {
            var subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new Runtime();
            if (natives != null)
                runtime.Metadata.Add(natives);

            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(subrutine));

            var actual = runtime.CallSubrutine("test");

            Assert.AreEqual(InjectionValueKind.String, actual.Kind, codeBlock);
            Assert.AreEqual(expected, actual.String, codeBlock);
        }

        public static void TestSubrutine(int expected, string subrutineName, string file)
        {
            var runtime = new Runtime();
            var parser = new Parser();
            parser.AddErrorListener(new FailTestErrorListener());
            runtime.Load(parser.ParseFile(file));

            var actual = runtime.CallSubrutine(subrutineName);

            Assert.AreEqual(InjectionValueKind.Integer, actual.Kind, file);
            Assert.AreEqual(expected, actual.Integer, file);
        }
    }
}
