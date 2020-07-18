using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The ClassWithMembers record is less verbose than ClassWithMembersAndTypes. It does not contain information 
    /// about the Remoting Type information of the Members.This record can be used when the information is deemed 
    /// unnecessary because it is known out of band or can be inferred from context.
    ///
    /// [MS-NRBF] 2.3.2.2 ClassWithMembers
    /// </summary>
    internal class ClassWithMembers : RecordTypeStructure
    {
        public ClassInfo ClassInfo { get; }
        public int LibraryId { get; }

        public ClassWithMembers(DeserializeReader reader) : base(reader, RecordType.ClassWithMembers)
        {
            ClassInfo = new ClassInfo(reader);
            LibraryId = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"ClassInfo = {{ {ClassInfo} }}; LibraryId = {LibraryId}";
        }
    }
}