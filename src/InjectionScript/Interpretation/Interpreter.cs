using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InjectionScript.Interpretation
{
    public class Interpreter : injectionBaseVisitor<InjectionValue>
    {
        private struct ConditionalGoto
        {
            public int TriggerIndex { get; }
            public int TargetIndex { get; }

            public ConditionalGoto(int trigger, int target)
            {
                TriggerIndex = trigger;
                TargetIndex = target;
            }
        }

        private readonly Metadata metadata;
        private readonly SemanticScope semanticScope = new SemanticScope();

        public Interpreter(Metadata metadata) => this.metadata = metadata;

        public InjectionValue CallSubrutine(injectionParser.SubrutineContext subrutine)
            => CallSubrutine(subrutine, Array.Empty<InjectionValue>());

        public InjectionValue CallSubrutine(injectionParser.SubrutineContext subrutine, InjectionValue[] argumentValues)
        {
            var forScopes = new Stack<ForScope>();
            var repeatIndexes = new Stack<int>();
            var whileIndexes = new Stack<int>();
            var conditionalGotos = new Stack<ConditionalGoto>();
            var flattener = new StatementFlattener();
            flattener.Visit(subrutine);
            var statementsMap = flattener.Statements;

            semanticScope.Start();
            var parameters = subrutine.parameters()?.parameterName()?.Select(x => x.SYMBOL().GetText()).ToArray()
                ?? Array.Empty<string>();
            for (int i = 0; i < parameters.Length; i++)
                semanticScope.DefineVar(parameters[i], argumentValues[i]);

            try
            {
                var statementIndex = 0;
                while (statementIndex < statementsMap.Count)
                {
                    var statement = statementsMap.GetStatement(statementIndex);

                    try
                    {
                        if (statement.returnStatement() != null)
                        {
                            return Visit(statement.returnStatement());
                        }
                        else if (statement.@if() != null)
                        {
                            injectionParser.StatementContext nextStatement;

                            var condition = Visit(statement.@if().expression());
                            if (condition != InjectionValue.False)
                            {
                                nextStatement = (injectionParser.StatementContext)statement.@if().Parent;
                                var triggerStatement = statement.@if().@else()?.codeBlock()?.statement()?.FirstOrDefault();
                                var targetStatement = statement.@if().@else()?.codeBlock()?.statement()?.LastOrDefault();
                                if (triggerStatement != null && targetStatement != null)
                                {
                                    var conditionalGoto = new ConditionalGoto(statementsMap.GetIndex(triggerStatement),
                                        statementsMap.GetIndex(targetStatement) + 1);
                                    conditionalGotos.Push(conditionalGoto);
                                }
                            }
                            else
                                nextStatement = statement.@if().codeBlock()?.statement()?.LastOrDefault();

                            if (nextStatement != null)
                                statementIndex = statementsMap.GetIndex(nextStatement) + 1;
                            else
                                statementIndex++;
                        }
                        else if (statement.@for() != null)
                        {
                            statementIndex++;
                            forScopes.Push(InterpretFor(statementIndex, statement.@for()));
                        }
                        else if (statement.next() != null)
                        {
                            var forScope = forScopes.Peek();
                            if (!semanticScope.TryGetVar(forScope.VariableName, out var variable))
                                throw new ScriptFailedException($"Variable undefined - {forScope.VariableName}", statementsMap.GetStatement(statementIndex).Start.Line);

                            variable = variable + new InjectionValue(1);
                            semanticScope.SetVar(forScope.VariableName, variable);
                            if (variable < forScope.Range)
                            {
                                statementIndex = forScope.StatementIndex;
                            }
                            else
                                statementIndex++;
                        }
                        else if (statement.repeat() != null)
                        {
                            statementIndex++;
                            repeatIndexes.Push(statementIndex);
                        }
                        else if (statement.until() != null)
                        {
                            var condition = Visit(statement.until().expression());
                            if (condition != InjectionValue.False)
                            {
                                statementIndex++;
                                repeatIndexes.Pop();
                            }
                            else
                                statementIndex = repeatIndexes.Peek();
                        }
                        else if (statement.@while() != null)
                        {
                            var condition = Visit(statement.@while().expression());
                            if (condition == InjectionValue.False)
                            {
                                while (statementIndex < statementsMap.Count && statementsMap.GetStatement(statementIndex).wend() == null)
                                {
                                    statementIndex++;
                                }
                                if (statementsMap.GetStatement(statementIndex).wend() != null)
                                {
                                    statementIndex++;
                                    if (whileIndexes.Any())
                                        whileIndexes.Pop();
                                }
                                else
                                    throw new NotImplementedException();
                            }
                            else
                            {
                                whileIndexes.Push(statementIndex);
                                statementIndex++;
                            }
                        }
                        else if (statement.wend() != null)
                        {
                            statementIndex = whileIndexes.Peek();
                        }
                        else if (statement.@goto() != null)
                        {
                            statementIndex = statementsMap.GetIndex(statement.@goto().SYMBOL().GetText());
                        }
                        else
                        {
                            Visit(statement);
                            statementIndex++;
                        }

                        while (conditionalGotos.Count > 0 && conditionalGotos.Peek().TriggerIndex == statementIndex)
                        {
                            var conditionalGoto = conditionalGotos.Pop();
                            statementIndex = conditionalGoto.TargetIndex;
                        }
                    }
                    catch (StatementFailedException ex)
                    {
                        throw new ScriptFailedException(ex.Message, statement.Start.Line, ex);
                    }
                }

                return InjectionValue.Unit;
            }
            finally
            {
                semanticScope.End();
            }
        }

        private ForScope InterpretFor(int statementIndex, injectionParser.ForContext forContext)
        {
            var variableName = forContext.assignment().lvalue().SYMBOL().GetText();
            Visit(forContext.assignment());
            var range = Visit(forContext.expression());

            return new ForScope(variableName, range, statementIndex);
        }

        public override InjectionValue VisitVarDef([NotNull] injectionParser.VarDefContext context)
        {
            if (context.assignment() != null)
            {
                if (context.assignment().lvalue().SYMBOL() != null)
                {
                    semanticScope.DefineVar(context.assignment().lvalue().SYMBOL().GetText());
                }
                else
                    throw new NotImplementedException();
                Visit(context.assignment());
            }
            else if (context.SYMBOL() != null)
                semanticScope.DefineVar(context.SYMBOL().GetText());


            return InjectionValue.Unit;
        }

        public override InjectionValue VisitDimDef([NotNull] injectionParser.DimDefContext context)
        {
            var name = context.SYMBOL().GetText();
            var limit = (int)Visit(context.expression());
            semanticScope.DefineDim(name, limit);

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitAssignment([NotNull] injectionParser.AssignmentContext context)
        {
            var value = Visit(context.expression());
            if (context.lvalue().indexedSymbol() != null)
            {
                var name = context.lvalue().indexedSymbol().SYMBOL().GetText();
                var index = (int)Visit(context.lvalue().indexedSymbol().expression());

                semanticScope.SetDim(name, index, value);
            }
            else
            {
                var name = context.lvalue().SYMBOL().GetText();
                semanticScope.SetVar(name, value);
            }

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitExpression([NotNull] injectionParser.ExpressionContext context)
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

        public override InjectionValue VisitLiteral([NotNull] injectionParser.LiteralContext context) => new InjectionValue(context.DOUBLEQUOTED_LITERAL()?.GetText().Trim('\"')
                ?? context.SINGLEQUOTED_LITERAL()?.GetText().Trim('\''));

        public override InjectionValue VisitLogicalOperand([NotNull] injectionParser.LogicalOperandContext context)
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
                    result = result > operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.EQUAL() != null)
                    result = result == operand ? InjectionValue.True : InjectionValue.False;
                else if (comparativeOperator.NOT_EQUAL() != null)
                    result = result != operand ? InjectionValue.True : InjectionValue.False;
                else
                    throw new NotImplementedException();
            }

            return result;
        }

        public override InjectionValue VisitComparativeOperand([NotNull] injectionParser.ComparativeOperandContext context)
        {
            var result = Visit(context.additiveOperand());

            foreach (var operation in context.additiveOperation())
            {
                var operand = Visit(operation.additiveOperand());
                var additiveOperator = operation.additiveOperator();

                switch (additiveOperator.GetText())
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

        public override InjectionValue VisitOperand([NotNull] injectionParser.OperandContext context)
        {
            var name = context.SYMBOL()?.GetText();
            if (!string.IsNullOrEmpty(name))
            {
                if (metadata.TryGetIntrinsicVariable(name, out var intrinsicVariable))
                    return intrinsicVariable.Call(Array.Empty<InjectionValue>());
                if (semanticScope.TryGetVar(name, out var variable))
                    return variable;

                throw new ScriptFailedException($"Variable undefined - {name}", context.Start.Line);
            }

            return base.VisitOperand(context);
        }

        public override InjectionValue VisitIndexedSymbol([NotNull] injectionParser.IndexedSymbolContext context)
        {
            var index = (int)Visit(context.expression());
            var name = context.SYMBOL().GetText();

            return semanticScope.GetDim(name, index);
        }

        public override InjectionValue VisitNumber([NotNull] injectionParser.NumberContext context)
        {
            if (context.HEX_NUMBER() != null)
            {
                var hex = context.HEX_NUMBER().GetText().Substring(2);
                return new InjectionValue(int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
            else if (context.INT_NUMBER() != null)
            {
                return new InjectionValue(int.Parse(context.INT_NUMBER().GetText(), CultureInfo.InvariantCulture));
            }
            else if (context.DEC_NUMBER() != null)
                return new InjectionValue(double.Parse(context.DEC_NUMBER().GetText(), CultureInfo.InvariantCulture));
            else
                throw new NotImplementedException();
        }

        public override InjectionValue VisitSubExpression([NotNull] injectionParser.SubExpressionContext context) => Visit(context.expression());

        public override InjectionValue VisitCodeBlock([NotNull] injectionParser.CodeBlockContext context)
        {
            if (context.statement() != null)
            {
                var statements = context.statement();
                for (var i = 0; i < statements.Length; i++)
                {
                    var statement = statements[i];
                    if (statement.@for() != null)
                        Visit(statement);
                }
            }

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitReturnStatement([NotNull] injectionParser.ReturnStatementContext context)
        {
            if (context.expression() != null)
                return Visit(context.expression());

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitCall([NotNull] injectionParser.CallContext context)
        {
            var name = context.SYMBOL().GetText();

            var argumentValues = context.argumentList().arguments()?.argument()?
                .Select(arg => VisitExpression(arg.expression()))
                .ToArray() ?? Array.Empty<InjectionValue>();

            if (metadata.TryGetNativeSubrutine(name, argumentValues, out var nativeSubrutine))
            {
                return nativeSubrutine.Call(argumentValues);
            }
            else
            {
                if (metadata.TryGetSubrutine(name, argumentValues.Length, out var customSubrutine))
                    return CallSubrutine(customSubrutine.Subrutine, argumentValues);
                else
                {
                    var argumentKinds = argumentValues.Any()
                        ? argumentValues.Select(x => x.Kind.ToString()).Aggregate((l, r) => l + "," + r)
                        : "no arguments";
                    throw new ScriptFailedException($"Function not found {name} ({argumentKinds})", context.Start.Line);
                }
            }
        }
    }
}
