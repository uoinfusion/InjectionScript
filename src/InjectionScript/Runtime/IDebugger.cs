namespace InjectionScript.Runtime
{
    public interface IDebugger
    {
        void BeforeStatement(StatementExecutionContext context);
        void BeforeVariableAssignment(VariableAssignmentContext context);
        void BeforeCall(CallContext context);
        void BeforeReturn(ReturnContext context);
    }
}
