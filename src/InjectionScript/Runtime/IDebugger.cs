namespace InjectionScript.Runtime
{
    public interface IDebugger
    {
        void BeforeStatement(StatementExecutionContext context);
    }
}
