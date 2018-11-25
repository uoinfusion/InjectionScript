using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public sealed class WhileInstruction : Instruction
    {
        public injectionParser.WhileContext WhileSyntax { get; }
        public override injectionParser.StatementContext Statement { get; }

        public int WendAddress { get; internal set; }

        public WhileInstruction(injectionParser.WhileContext whileSyntax)
        {
            WhileSyntax = whileSyntax;
            Statement = (injectionParser.StatementContext)whileSyntax.Parent;
        }
    }
}
