using InjectionScript.Parsing.Syntax;

namespace InjectionScript.Runtime.Contexts
{
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
