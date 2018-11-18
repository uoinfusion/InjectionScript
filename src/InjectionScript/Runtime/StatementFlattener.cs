using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class StatementFlattener : injectionBaseVisitor<bool>
    {
        private readonly List<injectionParser.StatementContext> statements
            = new List<injectionParser.StatementContext>();
        private readonly Dictionary<string, int> labels = new Dictionary<string, int>();
        private int lineNumber;

        public StatementMap Statements => new StatementMap(statements, labels);

        public override bool VisitStatement([NotNull] injectionParser.StatementContext context)
        {
            statements.Add(context);
            lineNumber++;

            return base.VisitStatement(context);
        }

        public override bool VisitLabel([NotNull] injectionParser.LabelContext context)
        {
            string labelName = context.SYMBOL().GetText();
            labels.Add(labelName, lineNumber);

            return base.VisitLabel(context);
        }
    }
}
