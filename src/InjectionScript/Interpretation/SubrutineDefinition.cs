using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Interpretation
{
    public class SubrutineDefinition
    {
        public string Name { get; }
        internal injectionParser.SubrutineContext Subrutine { get; }

        internal SubrutineDefinition(string name, injectionParser.SubrutineContext subrutine)
        {
            Name = name;
            Subrutine = subrutine;
        }
    }
}
