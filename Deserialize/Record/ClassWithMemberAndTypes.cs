using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The ClassWithMembersAndTypes record is the most verbose of the Class records. It contains metadata about
    /// Members, including the names and Remoting Types of the Members.It also contains a Library ID that references
    /// the Library Name of the Class.
    ///
    /// [MS-NRBF] 2.3.2.1 ClassWithMembersAndTypes
    /// </summary>
    public class ClassWithMemberAndTypes : RecordTypeStructure
    {
        public ClassInfo ClassInfo { get; }
        public MemberTypeInfo MemberTypeInfo { get; }
        public int LibraryId { get; }

        public ClassWithMemberAndTypes(DeserializeReader reader) : base(reader, RecordType.ClassWithMembersAndTypes)
        {
            ClassInfo = new ClassInfo(reader);
            MemberTypeInfo = new MemberTypeInfo(reader, ClassInfo);
            LibraryId = reader.ReadInt32();

            if (MemberTypeInfo.Types.Count != ClassInfo.MemberCount)
            {
                throw new InvalidStructureException("Member type count mismatch");
            }
        }

        public override string ToString()
        {
            return $"LibraryId = {LibraryId}; ClassInfo = {{ {ClassInfo} }}; MemberTypeInfo = {{ {MemberTypeInfo} }}";
        }
    }
}