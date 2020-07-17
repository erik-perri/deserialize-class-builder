using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;
using System.IO;

namespace DeserializeClassBuilder.Deserialize.Record
{
    abstract public class RecordTypeStructure
    {
        public RecordType Type { get; }

        public RecordTypeStructure(DeserializeReader reader, RecordType expectedType)
        {
            Type = reader.ReadEnum<RecordType>();
            if (Type != expectedType)
            {
                throw new InvalidStructureException($"Expected {expectedType} but found {Type}");
            }
        }
    }
}