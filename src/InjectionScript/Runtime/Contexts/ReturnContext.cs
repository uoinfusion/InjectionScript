using InjectionScript.Parsing.Syntax;

namespace InjectionScript.Runtime.Contexts
{
    public sealed class ReturnContext
    {
        private readonly injectionParser.ReturnStatementContext context;
        private readonly InjectionValue value;

        public int Line => context.Start.Line;

        internal ReturnContext(injectionParser.ReturnStatementContext context, InjectionValue value)
        {
            this.context = context;
            this.value = value;
        }

        public override string ToString() => $"returning {value}";
    }
}