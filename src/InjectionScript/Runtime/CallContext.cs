using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public sealed class CallContext
    {
        private readonly injectionParser.CallContext context;
        private readonly string name;
        private readonly InjectionValue[] argumentValues;

        public int Line => context.Start.Line;

        internal CallContext(injectionParser.CallContext context, string name, InjectionValue[] argumentValues)
        {
            this.context = context;
            this.name = name;
            this.argumentValues = argumentValues;
        }

        public override string ToString()
        {
            var args = argumentValues.Any()
                ? argumentValues.Select(x => x.ToString()).Aggregate((l, r) => l + "," + r)
                : string.Empty;

            return $"calling {name}({args})";
        }
    }
}
