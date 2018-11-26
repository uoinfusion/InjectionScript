using InjectionScript.Runtime.Contexts;

namespace InjectionScript.Runtime
{
    public interface IDebugger
    {
        void BeforeStatement(StatementExecutionContext context);
        void BeforeVariableAssignment(VariableAssignmentContext context);
        void AfterCall(AfterCallContext context);
        void BeforeReturn(ReturnContext context);
    }
}
