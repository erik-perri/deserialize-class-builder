using System;
using System.Runtime.Serialization;

namespace DeserializeClassBuilder.Deserialize.Exceptions
{
    public class InvalidStructureException : DeserializeException
    {
        public InvalidStructureException()
        {
        }

        public InvalidStructureException(string message) : base(message)
        {
        }

        public InvalidStructureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidStructureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}