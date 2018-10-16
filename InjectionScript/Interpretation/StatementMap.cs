using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class StatementMap
    {
        private readonly injectionParser.StatementContext[] statements;
        private readonly Dictionary<injectionParser.StatementContext, int> statementToIndex;

        public StatementMap(IEnumerable<injectionParser.StatementContext> statements)
        {
            this.statements = statements.ToArray();

            statementToIndex = new Dictionary<injectionParser.StatementContext, int>();
            var index = 0;

            foreach (var statement in this.statements)
            {
                statementToIndex.Add(statement, index);
                index++;
            }
        }

        public int Count => statements.Length;

        public int GetIndex(injectionParser.StatementContext statement) => statementToIndex[statement];

        public injectionParser.StatementContext GetStatement(int index) => statements[index];
    }
}
