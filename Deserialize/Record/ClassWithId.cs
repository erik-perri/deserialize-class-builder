using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The ClassWithId record is the most compact. It has no metadata. It refers to metadata defined in 
    /// SystemClassWithMembers, SystemClassWithMembersAndTypes, ClassWithMembers, or ClassWithMembersAndTypes record.
    ///
    /// [MS-NRBF] 2.3.2.5 ClassWithId
    /// </summary>
    internal class ClassWithId : RecordTypeStructure
    {
        public int ObjectId { get; }
        public int MetadataId { get; }
        public int Unknown { get; }

        public ClassWithId(DeserializeReader reader) : base(reader, RecordType.ClassWithId)
        {
            ObjectId = reader.ReadInt32();
            MetadataId = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"ObjectId = {ObjectId}; MetadataId = {MetadataId}";
        }
    }
}