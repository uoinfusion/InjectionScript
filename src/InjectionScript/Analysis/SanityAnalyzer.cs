using InjectionScript.Runtime;
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

            var invalidSymbolVisitor = new InvalidSymbolVisitor(messages, metadata);
            invalidSymbolVisitor.Visit(fileContext);

            var subrutineDefinitionsVisitor = new SubrutineDefinitionsVisitor(messages);
            subrutineDefinitionsVisitor.Visit(fileContext);

            var nativeSubrutineIncorrectArgumentsVisitor = new NativeSubrutineIncorrectArgumentsVisitor(messages);
            nativeSubrutineIncorrectArgumentsVisitor.Visit(fileContext);

            var invalidBreakVisitor = new InvalidBreakVisitor(messages);
            invalidBreakVisitor.Visit(fileContext);

            return new MessageCollection(messages);
        }
    }

}
