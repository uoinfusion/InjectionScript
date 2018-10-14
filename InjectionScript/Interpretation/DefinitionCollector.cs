using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
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
            metadata.Add(new SubrutineDefinition(context.name.Text, context));

            return metadata;
        }
    }
}
