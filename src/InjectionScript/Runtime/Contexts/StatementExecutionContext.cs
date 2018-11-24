using System;
using InjectionScript.Parsing.Syntax;

namespace InjectionScript.Runtime.Contexts
{
    public sealed class StatementExecutionContext
    {
        private readonly injectionParser.StatementContext statement;
        private readonly Interpreter interpreter;

        public StatementExecutionContext(int statementIndex, int line, string file, injectionParser.StatementContext statement, Interpreter interpreter)
        {
            StatementIndex = statementIndex;
            Line = line;
            File = file;
            this.statement = statement;
            this.interpreter = interpreter;
        }

        public int StatementIndex { get; }
        public int Line { get; }
        public string File { get; }

        internal InjectionValue Eval(injectionParser.ExpressionContext expression)
            => interpreter.VisitExpression(expression);

        internal string GetStatementText() => statement?.GetText() ?? "<no statement>";
    }
}
