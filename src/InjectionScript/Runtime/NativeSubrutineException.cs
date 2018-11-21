using System;
using System.Runtime.Serialization;

namespace InjectionScript.Runtime
{
    [Serializable]
    internal class NativeSubrutineException : Exception
    {
        public NativeSubrutineException()
        {
        }

        public NativeSubrutineException(string message) : base(message)
        {
        }

        public NativeSubrutineException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NativeSubrutineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString() => InnerException.ToString();
    }
}