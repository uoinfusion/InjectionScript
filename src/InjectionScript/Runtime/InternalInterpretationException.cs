using System;

namespace InjectionScript.Runtime
{
    public class InternalInterpretationException : Exception
    {
        public int Line { get; }

        public InternalInterpretationException(int line)
            : this(line, null)
        {
        }

        public InternalInterpretationException(int line, Exception inner)
            : base("An unhandled exception was thrown during script execution.", inner)
        {
            Line = line;
        }
    }
}
