using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript
{
    internal static class ContextExtensions
    {
        public static string GetSubrutineKey(this injectionParser.SubrutineContext context)
        {
            var parameterCount = context.parameters()?.parameterName()?.Length ?? 0;
            var name = context.SYMBOL().GetText();

            return $"{name}`{parameterCount}";
        }
    }
}
