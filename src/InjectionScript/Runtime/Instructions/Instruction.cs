using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public abstract class Instruction
    {
        public abstract injectionParser.StatementContext Statement { get; }
    }
}
