using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class StatementFlattener : injectionBaseVisitor<bool>
    {
        private readonly List<injectionParser.StatementContext> statements
            = new List<injectionParser.StatementContext>();
        public StatementMap Statements => new StatementMap(statements);

        public override bool VisitStatement([NotNull] injectionParser.StatementContext context)
        {
            statements.Add(context);

            return base.VisitStatement(context);
        }

    }
}
