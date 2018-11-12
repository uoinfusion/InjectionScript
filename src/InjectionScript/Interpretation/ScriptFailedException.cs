using System;

namespace InjectionScript.Interpretation
{
    public class ScriptFailedException : Exception
    {
        public int Line { get; }

        public ScriptFailedException(string message, int line)
            : this(message, line, null)
        {
        }

        public ScriptFailedException(string message, int line, StatementFailedException inner)
            : base(message, inner)
        {
            Line = line;
        }
    }
}
