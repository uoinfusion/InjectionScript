using InjectionScript;
using InjectionScript.Runtime;
using System.Linq;

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

    public override string ToString()
    {
        if (Messages.Any())
            return Messages.ToString();

        if (Result.HasValue)
            return Result.Value.ToString();

        return "no messages, no value";
    }
}
