using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class GlobalVariableDefinition
    {
        public string Name { get; }
        public injectionParser.ExpressionContext InitialValueExpression { get; }
        public bool HasInitialValue => InitialValueExpression != null;

        public GlobalVariableDefinition(string name, injectionParser.ExpressionContext initialValueExpression)
        {
            Name = name;
            this.InitialValueExpression = initialValueExpression;
        }

        public GlobalVariableDefinition(string name)
        {
            Name = name;
            InitialValueExpression = null;
        }
    }
}
