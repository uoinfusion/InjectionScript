using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public sealed class GenericStatementInstruction : Instruction
    {
        public override injectionParser.StatementContext Statement { get; }

        public GenericStatementInstruction(injectionParser.StatementContext statement)
        {
            Statement = statement;
        }
    }
}
