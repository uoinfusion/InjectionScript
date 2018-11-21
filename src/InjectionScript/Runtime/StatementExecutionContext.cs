using InjectionScript.Parsing.Syntax;

namespace InjectionScript.Runtime
{
    public class StatementExecutionContext
    {
        private readonly Interpreter interpreter;

        public StatementExecutionContext(int statementIndex, int line, string file, Interpreter interpreter)
        {
            StatementIndex = statementIndex;
            Line = line;
            File = file;
            this.interpreter = interpreter;
        }

        public int StatementIndex { get; }
        public int Line { get; }
        public string File { get; }

        internal InjectionValue Eval(injectionParser.ExpressionContext expression)
            => interpreter.VisitExpression(expression);
    }
}
