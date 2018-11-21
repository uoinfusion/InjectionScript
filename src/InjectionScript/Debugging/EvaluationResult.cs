using InjectionScript;
using InjectionScript.Runtime;

public class EvaluationResult
{
    public MessageCollection Messages { get; }
    public InjectionValue? Result { get; }

    public EvaluationResult(Message message, InjectionValue? result)
        : this(new MessageCollection(message), result)
    {
    }

    public EvaluationResult(MessageCollection messages, InjectionValue? result)
    {
        Messages = messages;
        Result = result;
    }

    public EvaluationResult(InjectionValue result)
    {
        Result = result;
        Messages = MessageCollection.Empty;
    }
}
