using DeserializeClassBuilder.Deserialize.Enums;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The MemberReference record contains a reference to another record that contains the actual value. The record is
    /// used to serialize values of a Class Member and Array items.
    ///
    /// [MS-NRBF] 2.5.3 MemberReference
    /// </summary>
    internal class MemberReference : RecordTypeStructure
    {
        public int IdRef { get; }

        public MemberReference(DeserializeReader reader) : base(reader, RecordType.MemberReference)
        {
            IdRef = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"IdRef = {IdRef}";
        }
    }
}