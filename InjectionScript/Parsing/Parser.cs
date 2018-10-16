using Antlr4.Runtime;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Parsing
{
    public class Parser
    {
        private readonly List<BaseErrorListener> errorListeners = new List<BaseErrorListener>();

        public injectionParser.FileContext ParseFile(string file)
        {
            var inputStream = new AntlrInputStream(file);
            var lexer = new injectionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new injectionParser(tokenStream);
            foreach (var listener in errorListeners)
                parser.AddErrorListener(listener);

            return parser.file();
        }

        public injectionParser.ExpressionContext ParseExpression(string expression)
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new injectionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new injectionParser(tokenStream);
            foreach (var listener in errorListeners)
                parser.AddErrorListener(listener);

            return parser.expression();
        }

        public void AddErrorListener(BaseErrorListener errorListener) => errorListeners.Add(errorListener);
    }
}
