using Antlr4.Runtime.Misc;
using InjectionScript.Parsing;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InjectionScript.Runtime.Instructions
{
    public class Generator : injectionBaseVisitor<bool>
    {
        private readonly List<Instruction> instructions = new List<Instruction>();
        private int currentAddress;
        private readonly MultiValueDictionary<string, GotoInstruction> gotos
            = new MultiValueDictionary<string, GotoInstruction>();
        private readonly Dictionary<string, int> labels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        internal Instruction[] Instructions => instructions.ToArray();

        public override bool VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            base.VisitSubrutine(context);

            foreach (var key in gotos.Keys)
            {
                var labelIndex = labels[key];
                foreach (var g in gotos[key])
                    g.TargetAddress = labelIndex;
            }

            return true;
        }

        public override bool VisitStatement([NotNull] injectionParser.StatementContext context)
        {
            if (context.@goto() != null)
                Generate(context.@goto());
            else if (context.label() != null)
                Generate(context.label());
            else if (context.@if() != null)
                Generate(context.@if());
            else if (context.@while() != null)
                Generate(context.@while());
            else
                AddInstruction(new GenericStatementInstruction(context));

            return true;
        }

        private void Generate(injectionParser.WhileContext whileContext)
        {
            var whileInstruction = new WhileInstruction(whileContext);
            int whileAddress = currentAddress;
            AddInstruction(whileInstruction);

            foreach (var statement in whileContext.codeBlock().statement())
                VisitStatement(statement);

            AddInstruction(new JumpInstruction(whileAddress));

            whileInstruction.WendAddress = currentAddress;
        }

        private void Generate(injectionParser.LabelContext context)
        {
            var label = context.SYMBOL().GetText();
            labels.Add(label, currentAddress);
        }

        private void Generate(injectionParser.GotoContext context)
        {
            var label = context.SYMBOL().GetText();
            var gotoInstruction = new GotoInstruction(context);
            gotos.Add(label, gotoInstruction);
            AddInstruction(gotoInstruction);
        }

        private void Generate(injectionParser.IfContext ifContext)
        {
            var ifInstruction = new IfInstruction(ifContext);
            AddInstruction(ifInstruction);
            var endIfJump = new JumpInstruction();

            if (ifContext.codeBlock()?.statement() != null)
            {
                foreach (var statement in ifContext.codeBlock().statement())
                    VisitStatement(statement);
            }

            if (ifContext.@else()?.codeBlock()?.statement() != null)
            {
                AddInstruction(endIfJump);
                ifInstruction.ElseAddress = currentAddress;
                foreach (var statement in ifContext.@else().codeBlock().statement())
                    VisitStatement(statement);
            }

            ifInstruction.EndIfAddress = currentAddress;
            endIfJump.TargetAddress = currentAddress;
        }

        private void AddInstruction(Instruction instruction)
        {
            instructions.Add(instruction);
            currentAddress++;
        }
    }
}
