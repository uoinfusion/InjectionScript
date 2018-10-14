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
    public static class Parser
    {
        public static injectionParser.FileContext ParseFile(string fileName)
        {
            var inputStream = new AntlrInputStream(File.ReadAllText(fileName));
            var lexer = new injectionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new injectionParser(tokenStream);

            return parser.file();
        }

        public static injectionParser.ExpressionContext ParseExpression(string expression)
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new injectionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new injectionParser(tokenStream);

            return parser.expression();
        }
    }
}
