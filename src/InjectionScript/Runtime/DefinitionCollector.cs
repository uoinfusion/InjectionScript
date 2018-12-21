using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime.Instructions;

namespace InjectionScript.Runtime
{
    public class DefinitionCollector : injectionBaseVisitor<Metadata>
    {
        private readonly Metadata metadata;

        public DefinitionCollector(Metadata metadata)
        {
            this.metadata = metadata;
        }

        public override Metadata VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            if (!string.IsNullOrEmpty(context.name?.Text))
            {
                var generator = new Generator();
                generator.Visit(context);

                metadata.Add(new SubrutineDefinition(context.name.Text, context, generator.Instructions));
            }

            return metadata;
        }
    }
}
