using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public sealed class GotoInstruction : Instruction
    {
        public injectionParser.GotoContext GotoSyntax { get; }
        public int TargetAddress { get; internal set; }

        public override injectionParser.StatementContext Statement { get; }

        public GotoInstruction(injectionParser.GotoContext gotoSyntax)
        {
            GotoSyntax = gotoSyntax;
            Statement = (injectionParser.StatementContext)gotoSyntax.Parent;
        }
    }
}
