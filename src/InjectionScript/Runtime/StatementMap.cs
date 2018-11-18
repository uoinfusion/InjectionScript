using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class StatementMap
    {
        private readonly injectionParser.StatementContext[] statements;
        private readonly Dictionary<injectionParser.StatementContext, int> statementToIndex;
        private readonly Dictionary<string, int> labels;

        public StatementMap(IEnumerable<injectionParser.StatementContext> statements, Dictionary<string, int> labels)
        {
            this.statements = statements.ToArray();
            this.labels = labels;

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
        public int GetIndex(string labelName) => labels[labelName];

        public injectionParser.StatementContext GetStatement(int index) => statements[index];
    }
}
