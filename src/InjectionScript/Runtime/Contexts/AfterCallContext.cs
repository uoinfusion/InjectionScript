using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Contexts
{
    public sealed class AfterCallContext
    {
        private readonly injectionParser.CallContext context;
        private readonly string name;
        private readonly InjectionValue[] argumentValues;

        public int Line => context.Start.Line;

        public InjectionValue ReturnValue { get; }

        internal AfterCallContext(injectionParser.CallContext context, string name, InjectionValue[] argumentValues, InjectionValue returnValue)
        {
            this.context = context;
            this.name = name;
            this.argumentValues = argumentValues;
            ReturnValue = returnValue;
        }

        public override string ToString()
        {
            var args = argumentValues.Any()
                ? argumentValues.Select(x => x.ToString()).Aggregate((l, r) => l + "," + r)
                : string.Empty;

            return $"Line {Line}: called {name}({args}) -> {ReturnValue}";
        }
    }
}
