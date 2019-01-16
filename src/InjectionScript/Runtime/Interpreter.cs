using Antlr4.Runtime.Misc;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime.Contexts;
using InjectionScript.Runtime.Instructions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InjectionScript.Runtime
{
    public class Interpreter : injectionBaseVisitor<InjectionValue>
    {
        private readonly Metadata metadata;
        private readonly string currentFileName;
        private readonly IDebugger debugger;
        private readonly SemanticScope semanticScope = new SemanticScope();

        public Interpreter(Metadata metadata, string currentFileName, IDebugger debugger = null)
        {
            this.metadata = metadata;
            this.currentFileName = currentFileName;
            this.debugger = debugger;
        }

        public InjectionValue CallSubrutine(injectionParser.SubrutineContext subrutine)
            => CallSubrutine(subrutine, Array.Empty<InjectionValue>());

        public InjectionValue CallSubrutine(injectionParser.SubrutineContext subrutine, InjectionValue[] argumentValues)
        {
            var forScopes = new Stack<ForScope>();
            var repeatIndexes = new Stack<int>();

            semanticScope.Start();
            var parameters = subrutine.parameters()?.parameterName()?.Select(x => x.SYMBOL().GetText()).ToArray()
                ?? Array.Empty<string>();
            for (int i = 0; i < parameters.Length; i++)
                semanticScope.DefineVar(parameters[i], argumentValues[i]);

            semanticScope.DefineGlobalVariables(metadata.GlobalVariables, globalVar => VisitExpression(globalVar.InitialValueExpression));

            var name = subrutine.subrutineName().GetText();
            var subrutineDefinition = metadata.GetSubrutine(name, parameters.Length);

            try
            {
                var instructionAddress = 0;
                while (instructionAddress < subrutineDefinition.Instructions.Length)
                {
                    var currentInstruction = subrutineDefinition.Instructions[instructionAddress];
                    var statement = currentInstruction.Statement;

                    if (statement == null)
                    {
                        switch (currentInstruction)
                        {
                            case JumpInstruction jump:
                                instructionAddress = jump.TargetAddress;
                                break;
                        }

                        continue;
                    }

                    if (debugger != null)
                    {
                        var context = new StatementExecutionContext(instructionAddress, statement.Start.Line, currentFileName, statement, this);
                        debugger.BeforeStatement(context);
                    }

                    try
                    {
                        if (statement.returnStatement() != null)
                        {
                            return Visit(statement.returnStatement());
                        }
                        else if (statement.@if() != null)
                        {
                            var condition = Visit(statement.@if().expression());
                            if (condition == InjectionValue.False)
                            {
                                var ifInstruction = (IfInstruction)currentInstruction;
                                instructionAddress = ifInstruction.ElseAddress ?? ifInstruction.EndIfAddress;
                            }
                            else
                                instructionAddress++;
                        }
                        else if (statement.@for() != null)
                        {
                            instructionAddress++;
                            forScopes.Push(InterpretFor(instructionAddress, statement.@for(), semanticScope));
                        }
                        else if (statement.next() != null)
                        {
                            var forScope = forScopes.Peek();
                            if (!semanticScope.TryGetVar(forScope.VariableName, out var variable))
                                throw new ScriptFailedException($"Variable undefined - {forScope.VariableName}", subrutineDefinition.Instructions[instructionAddress].Statement.Start.Line);

                            if (variable < forScope.Range)
                            {
                                instructionAddress = forScope.StatementIndex;
                                variable = variable + forScope.Step;
                                semanticScope.SetVar(forScope.VariableName, variable);
                            }
                            else
                            {
                                instructionAddress++;
                                forScopes.Pop();
                            }
                        }
                        else if (statement.repeat() != null)
                        {
                            instructionAddress++;
                            repeatIndexes.Push(instructionAddress);
                        }
                        else if (statement.until() != null)
                        {
                            var condition = Visit(statement.until().expression());
                            if (condition != InjectionValue.False)
                            {
                                instructionAddress++;
                                repeatIndexes.Pop();
                            }
                            else
                                instructionAddress = repeatIndexes.Peek();
                        }
                        else if (statement.@while() != null)
                        {
                            var whileInstruction = (WhileInstruction)currentInstruction;
                            var condition = Visit(statement.@while().expression());

                            if (condition != InjectionValue.False)
                                instructionAddress++;
                            else
                                instructionAddress = whileInstruction.WendAddress;
                        }
                        else if (statement.@goto() != null)
                        {
                            var gotoInstruction = (GotoInstruction)currentInstruction;
                            instructionAddress = gotoInstruction.TargetAddress;
                        }
                        else
                        {
                            Visit(statement);
                            instructionAddress++;
                        }
                    }
                    catch (StatementFailedException ex)
                    {
                        throw new ScriptFailedException(ex.Message, statement.Start.Line, ex);
                    }
                    catch (ScriptFailedException)
                    {
                        throw;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new ScriptFailedException(ex.Message, statement.Start.Line, ex);
                    }
                }

                if (debugger != null)
                {
                    debugger.BeforeStatement(
                        new StatementExecutionContext(instructionAddress, subrutine.END_SUB().Symbol.Line, currentFileName, null, this));
                }

                return InjectionValue.Unit;
            }
            finally
            {
                semanticScope.End();
            }
        }

        private ForScope InterpretFor(int statementIndex, injectionParser.ForContext forContext, SemanticScope scope)
        {
            var variableName = forContext.assignment()?.lvalue()?.SYMBOL()?.GetText() 
                ?? forContext.forVarDef()?.assignment()?.lvalue()?.SYMBOL()?.GetText();

            if (forContext.forVarDef() != null)
            {
                scope.DefineVar(variableName);
                Visit(forContext.forVarDef().assignment());
            }
            else
                Visit(forContext.assignment());

            var range = Visit(forContext.expression());
            InjectionValue step;
            if (forContext.step()?.expression() != null)
                step = Visit(forContext.step().expression());
            else
                step = new InjectionValue(1);

            return new ForScope(variableName, range, statementIndex, step);
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

                if (debugger != null)
                    debugger.BeforeVariableAssignment(new IndexedVariableAssignmentContex(context, currentFileName, name, value, index));
                semanticScope.SetDim(name, index, value);
            }
            else
            {
                var name = context.lvalue().SYMBOL().GetText();

                if (debugger != null)
                    debugger.BeforeVariableAssignment(new VariableAssignmentContext(context, currentFileName, name, value));

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
            {
                var result = Visit(context.expression());
                if (debugger != null)
                    debugger.BeforeReturn(new ReturnContext(context, result));

                if (result.Kind == InjectionValueKind.Array)
                    throw new StatementFailedException("Cannot return dim from a subrutine.");

                return result;
            }

            return InjectionValue.Unit;
        }

        public override InjectionValue VisitCall([NotNull] injectionParser.CallContext context)
        {
            var name = context.SYMBOL().GetText();

            var argumentValues = context.argumentList().arguments()?.argument()?
                .Select(arg => VisitExpression(arg.expression()))
                .ToArray() ?? Array.Empty<InjectionValue>();

            var result = InjectionValue.Unit;

            if (metadata.TryGetNativeSubrutine(name, argumentValues, out var nativeSubrutine))
            {
                result = nativeSubrutine.Call(argumentValues);
            }
            else
            {
                if (metadata.TryGetSubrutine(name, argumentValues.Length, out var customSubrutine))
                    result = CallSubrutine(customSubrutine.Syntax, argumentValues);
                else
                {
                    var argumentKinds = argumentValues.Any()
                        ? argumentValues.Select(x => x.Kind.ToString()).Aggregate((l, r) => l + "," + r)
                        : "no arguments";
                    throw new ScriptFailedException($"Function not found {name} ({argumentKinds})", context.Start.Line);
                }
            }

            if (debugger != null)
                debugger.AfterCall(new AfterCallContext(context, name, argumentValues, result));

            return result;
        }
    }
}
