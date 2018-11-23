using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
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

    internal sealed class IndexedVariableAssignmentContex : VariableAssignmentContext
    {
        public IndexedVariableAssignmentContex(injectionParser.AssignmentContext context, string file, string name, InjectionValue value, int index)
            : base(context, file, name, value)
        {
            Index = index;
        }

        public int Index { get; }

        public override string ToString() => $"Line {Line}: {Name}[{Index}] = {Value}";
    }
}
