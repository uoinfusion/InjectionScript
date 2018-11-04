using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Parsing
{
    internal sealed class MemorySyntaxErrorListener : BaseErrorListener
    {
        public IEnumerable<SyntaxError> Errors => errors;
        private readonly List<SyntaxError> errors = new List<SyntaxError>();

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            errors.Add(new SyntaxError(line, charPositionInLine, msg));
        }
    }
}
