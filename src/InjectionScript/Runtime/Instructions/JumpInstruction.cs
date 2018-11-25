using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InjectionScript.Parsing.Syntax;

namespace InjectionScript.Runtime.Instructions
{
    public class JumpInstruction : Instruction
    {
        public int TargetAddress { get; internal set; }

        public override injectionParser.StatementContext Statement => null;

        public JumpInstruction()
        {
        }

        public JumpInstruction(int targetAddress)
        {
            TargetAddress = targetAddress;
        }
    }
}
