using InjectionScript.Runtime.Contexts;
using System;

namespace InjectionScript.Runtime
{
    public interface IDebugger
    {
        void BeforeStatement(StatementExecutionContext context);
        void BeforeVariableAssignment(VariableAssignmentContext context);
        void AfterCall(AfterCallContext context);
        void BeforeReturn(ReturnContext context);
        void ExecutionFailed(Exception ex);
    }
}
