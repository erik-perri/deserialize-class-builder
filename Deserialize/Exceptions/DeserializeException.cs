using System;
using System.Runtime.Serialization;

namespace DeserializeClassBuilder.Deserialize.Exceptions
{
    internal class DeserializeException : Exception
    {
        public DeserializeException()
        {
        }

        public DeserializeException(string message) : base(message)
        {
        }

        public DeserializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeserializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}