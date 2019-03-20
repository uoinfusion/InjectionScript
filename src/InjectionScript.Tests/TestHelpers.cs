using InjectionScript.Runtime;
using InjectionScript.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace InjectionScript.Tests
{
    public static class TestHelpers
    {
        public static InjectionValue EvalExpression(string expression)
        {
            var parser = new Parser();
            var expressionSyntax = parser.ParseExpression(expression, new FailTestErrorListener());
            var runtime = new InjectionRuntime();

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
            var runtime = new InjectionRuntime();
            if (natives != null)
                runtime.Metadata.Add(natives);
            if (intrinsicVariables != null)
                runtime.Metadata.AddIntrinsicVariables(intrinsicVariables);

            var parser = new Parser();
            runtime.Load(parser.ParseFile(subrutine, new FailTestErrorListener()));

            runtime.CallSubrutine("test");
        }

        public static InjectionValue Execute(string subName, string codeBlock)
        {
            var runtime = new InjectionRuntime();

            var parser = new Parser();
            runtime.Load(parser.ParseFile(codeBlock, new FailTestErrorListener()));

            return runtime.CallSubrutine(subName); 
        }

        public static void TestSubrutine(int expected, string codeBlock, NativeSubrutineDefinition[] natives = null, NativeSubrutineDefinition[] intrinsicVariables = null)
        {
            var subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new InjectionRuntime();
            if (natives != null)
                runtime.Metadata.Add(natives);
            if (intrinsicVariables != null)
                runtime.Metadata.AddIntrinsicVariables(intrinsicVariables);

            var parser = new Parser();
            runtime.Load(parser.ParseFile(subrutine, new FailTestErrorListener()));

            var actual = runtime.CallSubrutine("test");

            Assert.AreEqual(InjectionValueKind.Integer, actual.Kind, codeBlock);
            Assert.AreEqual(expected, actual.Integer, codeBlock);
        }

        public static void TestSubrutine(string expected, string codeBlock, NativeSubrutineDefinition[] natives = null)
        {
            var subrutine = $"sub test()\r\n{codeBlock}\r\n end sub";
            var runtime = new InjectionRuntime();
            if (natives != null)
                runtime.Metadata.Add(natives);

            var parser = new Parser();
            runtime.Load(parser.ParseFile(subrutine, new FailTestErrorListener()));

            var actual = runtime.CallSubrutine("test");

            Assert.AreEqual(InjectionValueKind.String, actual.Kind, codeBlock);
            Assert.AreEqual(expected, actual.String, codeBlock);
        }

        public static Task<InjectionValue> RunSubrutine(string subrutineName, Func<CancellationToken?> retrieveCancellationToken, string file)
        {
            return Task.Run(() =>
            {
                var runtime = new InjectionRuntime(retrieveCancellationToken);
                var parser = new Parser();
                runtime.Load(parser.ParseFile(file, new FailTestErrorListener()));

                return runtime.CallSubrutine(subrutineName);
            });
        }

        public static void TestSubrutine(int expected, string subrutineName, string file)
        {
            var runtime = new InjectionRuntime();
            var parser = new Parser();
            runtime.Load(parser.ParseFile(file, new FailTestErrorListener()));

            var actual = runtime.CallSubrutine(subrutineName);

            Assert.AreEqual(InjectionValueKind.Integer, actual.Kind, file);
            Assert.AreEqual(expected, actual.Integer, file);
        }

        public static MessageCollection Parse(string fileContent)
        {
            var runtime = new InjectionRuntime();
            return runtime.Load(fileContent, "test.sc");
        }

        public static void AssertWarning(this MessageCollection collection, int line, string code)
        {
            AssertMessage(collection, MessageSeverity.Warning, "warning", line, code);
        }

        public static void AssertError(this MessageCollection collection, int line, string code)
        {
            AssertMessage(collection, MessageSeverity.Error, "error", line, code);
        }

        private static void AssertMessage(MessageCollection collection, MessageSeverity severity, string severityText, int line, string code)
        {
            if (!collection.Any(x => x.Severity == severity))
                Assert.Fail($"No {severityText} found.");

            if (!collection.Any(x => x.Severity == severity && x.StartLine == line))
                Assert.Fail($"No {severityText} found on line {line}.");

            if (!collection.Any(x => x.Severity == severity && x.StartLine == line && x.IsCode(code)))
                Assert.Fail($"No {severityText} found on line {line} with code {code}.");

            Assert.IsTrue(collection.Any(m => m.Severity == severity && m.StartLine == line && m.IsCode(code)));
        }

        public static void AssertNoWarning(this MessageCollection collection, int line, string code)
        {
            var message = collection.FirstOrDefault(x => x.Severity == MessageSeverity.Warning && x.StartLine == line && x.IsCode(code));

            if (message != null)
                Assert.Fail($"No warning expected on line {line}, but found:\n{message}");
        }

        public static void AssertNoError(this MessageCollection collection)
        {
            var errorCount = collection.Count(m => m.Severity == MessageSeverity.Error);
            if (errorCount > 0)
                Assert.Fail($"{errorCount} error(s) found\n{collection}");
        }

        public static void AssertNoWarning(this MessageCollection collection)
        {
            var warningCount = collection.Count(m => m.Severity == MessageSeverity.Warning);
            if (warningCount > 0)
                Assert.Fail($"{warningCount} warning(s) found\n{collection}");
        }
    }
}
