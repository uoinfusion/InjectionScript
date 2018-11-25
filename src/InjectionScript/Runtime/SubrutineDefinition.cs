using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime
{
    public class SubrutineDefinition
    {
        public string Name { get; }
        internal injectionParser.SubrutineContext Syntax { get; }
        internal Instruction[] Instructions { get; }

        internal SubrutineDefinition(string name, injectionParser.SubrutineContext subrutine, Instruction[] instructions)
        {
            Name = name;
            Syntax = subrutine;
            Instructions = instructions;
        }
    }
}
