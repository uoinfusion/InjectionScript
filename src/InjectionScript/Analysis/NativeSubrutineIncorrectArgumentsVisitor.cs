using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Analysis
{
    public class NativeSubrutineIncorrectArgumentsVisitor : injectionBaseVisitor<bool>
    {
        private readonly List<Message> messages;
        private readonly static MultiValueDictionary<string, int> argumentsCount = new MultiValueDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        static NativeSubrutineIncorrectArgumentsVisitor()
        {
            argumentsCount.Add("uo.findcount", 0);
        }

        public NativeSubrutineIncorrectArgumentsVisitor(List<Message> messages)
        {
            this.messages = messages;
        }

        public override bool VisitCall([NotNull] injectionParser.CallContext context)
        {
            var name = context.SYMBOL().GetText();
            var argumentCount = context.argumentList().arguments()?.argument()?.Count() ?? 0;

            if (argumentsCount.TryGet(name, out var values) && !values.Contains(argumentCount))
            {
                messages.Add(new Message(context.Start.Line, context.Start.Column, context.Stop.Line, context.Stop.Column,
                    MessageSeverity.Warning, MessageCodes.UndefinedSubrutine,
                    $"Invalid arguments count for '{name}'."));
            }

            return true;
        }

    }
}
