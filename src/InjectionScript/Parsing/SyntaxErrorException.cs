using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Parsing
{
    public sealed class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string fileName, IEnumerable<SyntaxError> errors)
        {
            FileName = fileName;
            Errors = errors.ToArray();
        }

        public string FileName { get; }
        public SyntaxError[] Errors { get; }
    }
}
