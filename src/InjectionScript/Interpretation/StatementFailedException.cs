using System;
using System.Runtime.Serialization;

namespace InjectionScript.Interpretation
{
    [Serializable]
    public class StatementFailedException : Exception
    {
        public StatementFailedException()
        {
        }

        public StatementFailedException(string message) : base(message)
        {
        }

        public StatementFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StatementFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}