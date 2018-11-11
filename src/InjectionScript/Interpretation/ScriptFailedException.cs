using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class ScriptFailedException : Exception
    {
        public ScriptFailedException(string message, int line)
        {
            Message = message;
            Line = line;
        }

        public string Message { get; }
        public int Line { get; }
    }
}
