using System;

namespace InjectionScript.Runtime
{
    public class ScriptFailedException : Exception
    {
        public int Line { get; }

        public ScriptFailedException(string message, int line)
            : this(message, line, null)
        {
        }

        public ScriptFailedException(string message, int line, Exception inner)
            : base(message, inner)
        {
            Line = line;
        }
    }
}
