using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Analysis
{
    internal class SubrutineDefinitionsVisitor : injectionBaseVisitor<bool>
    {
        private readonly MultiValueDictionary<string, injectionParser.SubrutineContext> subrutines =
            new MultiValueDictionary<string, injectionParser.SubrutineContext>();
        private List<Message> messages;

        public SubrutineDefinitionsVisitor(List<Message> messages)
        {
            this.messages = messages;
        }

        public override bool VisitFile([NotNull] injectionParser.FileContext context)
        {
            base.VisitFile(context);

            foreach (var valueCollection in subrutines.Values)
            {
                if (valueCollection.Count() > 1)
                {
                    var collection = valueCollection.OrderBy(v => v.Start.Line);
                    var firstValue = collection.First();

                    messages.Add(new Message(firstValue.Start.Line, firstValue.Start.Column, firstValue.Stop.Line, firstValue.Stop.Column,
                        MessageSeverity.Warning, MessageCodes.SubrutineRedefined,
                        $"Subrutine '{firstValue.SYMBOL().GetText()}' was redefined by later subrutine definitions with the same name."));

                    foreach (var value in collection.Skip(1))
                    {
                        messages.Add(new Message(value.Start.Line, value.Start.Column, value.Stop.Line, value.Stop.Column,
                            MessageSeverity.Warning, MessageCodes.SubrutineRedefinition,
                            $"Subrutine '{value.SYMBOL().GetText()}' redefines a subrutine with the same name."));
                    }
                }
            }

            return true;
        }

        public override bool VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            subrutines.Add(context.GetSubrutineKey(), context);

            return true;
        }
    }
}
