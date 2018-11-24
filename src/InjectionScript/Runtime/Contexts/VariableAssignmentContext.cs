using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Contexts
{
    public class VariableAssignmentContext
    {
        private readonly injectionParser.AssignmentContext context;

        public int Line => context.Start.Line;
        public string File { get; }
        public string Name { get; }
        public InjectionValue Value { get; }

        public VariableAssignmentContext(injectionParser.AssignmentContext context, string file, string name, InjectionValue value)
        {
            this.context = context;
            File = file;
            Name = name;
            Value = value;
        }

        public override string ToString() => $"Line {Line}: {Name} = {Value}";
    }
}
