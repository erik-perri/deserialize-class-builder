using DeserializeClassBuilder.Deserialize;
using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The SystemClassWithMembersAndTypes record is less verbose than ClassWithMembersAndTypes. It does not contain a
    /// LibraryId. This record implicitly specifies that the Class is in the System Library.
    ///
    /// [MS-NRBF] 2.3.2.3 SystemClassWithMembersAndTypes
    /// </summary>
    internal class SystemClassWithMembersAndTypes : RecordTypeStructure
    {
        public ClassInfo ClassInfo { get; }
        public MemberTypeInfo MemberTypeInfo { get; }

        public SystemClassWithMembersAndTypes(DeserializeReader reader) : base(reader, RecordType.SystemClassWithMembersAndTypes)
        {
            ClassInfo = new ClassInfo(reader);
            MemberTypeInfo = new MemberTypeInfo(reader, ClassInfo);

            if (MemberTypeInfo.Types.Count != ClassInfo.MemberCount)
            {
                throw new InvalidStructureException("Member type count mismatch");
            }
        }

        public override string ToString()
        {
            return $"ClassInfo = {{ {ClassInfo} }}; MemberTypeInfo = {{ {MemberTypeInfo} }}";
        }
    }
}