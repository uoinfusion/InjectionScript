using InjectionScript.Interpretation;
using InjectionScript.Parsing.Syntax;
using System.Collections.Generic;

namespace InjectionScript.Analysis
{
    public class SanityAnalyzer
    {
        public MessageCollection Analyze(injectionParser.FileContext fileContext, Metadata metadata)
        {
            var messages = new List<Message>();

            var extraneousEndifVisitor = new MisplacedStatementsVisitor(messages);
            extraneousEndifVisitor.Visit(fileContext);

            var unknownSymbolVisitor = new UnknownSymbolVisitor(messages, metadata);
            unknownSymbolVisitor.Visit(fileContext);

            return new MessageCollection(messages);
        }
    }

}
