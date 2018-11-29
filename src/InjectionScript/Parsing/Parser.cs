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
        private T Parse<T>(string sourceCode, BaseErrorListener errorListener, Func<injectionParser, T> parseFunc)
        {
            var inputStream = new AntlrInputStream(sourceCode);
            var lexer = new injectionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new injectionParser(tokenStream);
            if (errorListener != null)
                parser.AddErrorListener(errorListener);

            return parseFunc(parser);
        }

        private T Parse<T>(string sourceCode, Func<injectionParser, T> parseFunc, out MessageCollection messages)
        {
            var errorListener = new MemorySyntaxErrorListener();
            var result = Parse(sourceCode, errorListener, parseFunc);

            messages = new MessageCollection(errorListener.Errors);

            return result;
        }

        public injectionParser.FileContext ParseFile(string sourceCode, BaseErrorListener errorListener = null) 
            => Parse(sourceCode, errorListener, (parser) => parser.file());
        public injectionParser.FileContext ParseFile(string sourceCode, out MessageCollection messages) 
            => Parse(sourceCode, (parser) => parser.file(), out messages);

        public injectionParser.ExpressionContext ParseExpression(string sourceCode, BaseErrorListener errorListener = null)
            => Parse(sourceCode, errorListener, (parser) => parser.expression());
        public injectionParser.ExpressionContext ParseExpression(string sourceCode, out MessageCollection messages)
            => Parse(sourceCode, (parser) => parser.expression(), out messages);

        public injectionParser.StatementContext ParseStatement(string sourceCode, BaseErrorListener errorListener = null)
            => Parse(sourceCode, errorListener, (parser) => parser.statement());
    }
}
