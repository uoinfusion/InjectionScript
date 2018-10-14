using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Globalization;

namespace InjectionScript.Interpretation
{
    public class Interpreter : injectionBaseVisitor<InjectionValue>
    {
        private readonly Metadata metadata;

        public Interpreter(Metadata metadata)
        {
            this.metadata = metadata;
        }

        public override InjectionValue VisitSubrutine([NotNull] injectionParser.SubrutineContext context)
        {
            return Visit(context.codeBlock());
        }

        public override InjectionValue VisitExpression([NotNull] injectionParser.ExpressionContext context)
        {
            var result = Visit(context.comparativeOperand());

            foreach (var operation in context.comparativeOperation())
            {
                var operand = Visit(operation.comparativeOperand());
                var comparativeOperator = operation.comparativeOperator();

                if (comparativeOperator.LESS_THAN() != null)
                    result = result <= operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.LESS_THAN_STRICT() != null)
                    result = result < operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.MORE_THAN() != null)
                    result = result >= operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.MORE_THAN_STRICT() != null)
                    result = result < operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.EQUAL() != null)
                    result = result == operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.NOT_EQUAL() != null)
                    result = result != operand ? InjectionValue.True : InjectionValue.False;
                else
                    throw new NotImplementedException();
            }

            return result;
        }

        public override InjectionValue VisitLogicalOperand([NotNull] injectionParser.LogicalOperandContext context)
        {
            var result = Visit(context.additiveOperand());

            foreach (var operation in context.additiveOperation())
            {
                var operand = Visit(operation.additiveOperand());

                switch (operation.additiveOperator().GetText())
                {
                    case "+":
                        result = result + operand;
                        break;
                    case "-":
                        result = result - operand;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return result;
        }

        public override InjectionValue VisitComparativeOperand([NotNull] injectionParser.ComparativeOperandContext context)
        {
            var result = Visit(context.logicalOperand());

            foreach (var operation in context.logicalOperation())
            {
                var operand = Visit(operation.logicalOperand());
                var logicalOperator = operation.logicalOperator();

                if (logicalOperator.AND() != null)
                    result = result & operand;
                else if (logicalOperator.OR() != null)
                    result = result | operand;
                else
                    throw new NotImplementedException();
            }

            return result;
        }

        public override InjectionValue VisitAdditiveOperand([NotNull] injectionParser.AdditiveOperandContext context)
        {
            var result = Visit(context.signedOperand());

            foreach (var operation in context.multiplicativeOperation())
            {
                var operand = Visit(operation.signedOperand());

                switch (operation.multiplicativeOperator().GetText())
                {
                    case "/":
                        result = result / operand;
                        break;
                    case "*":
                        result = result * operand;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return result;
        }

        public override InjectionValue VisitSignedOperand([NotNull] injectionParser.SignedOperandContext context)
        {
            if (context.operand() != null)
                return Visit(context.operand());

            var operand = Visit(context.signedOperand());
            if (context.unaryOperator().MINUS() != null)
                return InjectionValue.MinusOne * operand;
            else if (context.unaryOperator().NOT() != null)
                return operand != InjectionValue.Zero ? InjectionValue.False : InjectionValue.True;
            else
                throw new NotImplementedException();
        }

        public override InjectionValue VisitNumber([NotNull] injectionParser.NumberContext context)
        {
            if (context.HEX_NUMBER() != null)
            {
                var hex = context.HEX_NUMBER().GetText().Substring(2);
                return new InjectionValue(int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
            else if (context.DEC_NUMBER() != null)
            {
                return new InjectionValue(int.Parse(context.DEC_NUMBER().GetText(), CultureInfo.InvariantCulture));
            }
            else
                throw new NotImplementedException();
        }

        public override InjectionValue VisitSubExpression([NotNull] injectionParser.SubExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override InjectionValue VisitCodeBlock([NotNull] injectionParser.CodeBlockContext context)
        {
            if (context.statement() != null)
            {
                foreach (var statement in context.statement())
                {
                    if (statement.returnStatement() != null)
                        return Visit(statement.returnStatement());
                    else
                        Visit(statement);
                }
            }

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitStatement([NotNull] injectionParser.StatementContext context)
        {
            if (context.returnStatement() != null)
                throw new NotImplementedException();

            throw new NotImplementedException();
        }
    }
}
