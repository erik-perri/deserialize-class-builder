using DeserializeClassBuilder.Deserialize.Enums;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The BinaryObjectString record identifies an object as a String object, and contains information about it.
    ///
    /// [MS-NRBF] 2.5.7 BinaryObjectString
    /// </summary>
    internal class BinaryObjectString : RecordTypeStructure
    {
        public int ObjectId { get; }
        public string Value { get; }

        public BinaryObjectString(DeserializeReader reader) : base(reader, RecordType.BinaryObjectString)
        {
            ObjectId = reader.ReadInt32();
            Value = reader.ReadString();
        }

        public override string ToString()
        {
            return $"ObjectId = {ObjectId}; Value = {Value}";
        }
    }
}