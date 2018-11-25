using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public sealed class IfInstruction : Instruction
    {
        public injectionParser.IfContext IfSyntax { get; }
        public int EndIfAddress { get; internal set; }
        public int? ElseAddress { get; internal set; }

        public override injectionParser.StatementContext Statement { get; }

        public IfInstruction(injectionParser.IfContext ifSyntax)
        {
            IfSyntax = ifSyntax;
            Statement = (injectionParser.StatementContext)ifSyntax.Parent;
        }
    }
}
