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
            if (!string.IsNullOrEmpty(context.subrutineName()?.GetText()))
            {
                var generator = new Generator();
                generator.Visit(context);

                metadata.Add(new SubrutineDefinition(context.subrutineName().GetText(), context, generator.Instructions));
            }

            return metadata;
        }

        public override Metadata VisitFileSection([NotNull] injectionParser.FileSectionContext context)
        {
            if (context.var()?.varDef() != null)
            {
                foreach (var varDef in context.var().varDef())
                {
                    if (varDef.assignment() != null)
                    {
                        if (varDef.assignment()?.lvalue() != null && varDef.assignment()?.expression() != null)
                        {
                            var name = varDef.assignment()?.lvalue().SYMBOL().GetText();
                            metadata.Add(new GlobalVariableDefinition(name, varDef.assignment()?.expression()));
                        }
                    }
                    else
                        metadata.Add(new GlobalVariableDefinition(varDef.SYMBOL().GetText()));
                }

                return metadata;
            }

            return base.VisitFileSection(context);
        }
    }
}
