using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class ForScope
    {
        public string VariableName { get; }
        public InjectionValue Range { get; }
        public int StatementIndex { get; }

        public ForScope(string variableName, InjectionValue range, int statementIndex)
        {
            VariableName = variableName;
            Range = range;
            StatementIndex = statementIndex;
        }
    }
}
