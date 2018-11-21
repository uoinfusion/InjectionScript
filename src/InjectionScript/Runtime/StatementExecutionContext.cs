namespace InjectionScript.Runtime
{
    public class StatementExecutionContext
    {
        public StatementExecutionContext(int statementIndex, int line, string file, Interpreter interpreter)
        {
            StatementIndex = statementIndex;
            Line = line;
            File = file;
            Interpreter = interpreter;
        }

        public int StatementIndex { get; }
        public int Line { get; }
        public string File { get; }
        public Interpreter Interpreter { get; }
    }
}
