using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;
using System.IO;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The SystemClassWithMembers record is less verbose than ClassWithMembersAndTypes. It does not contain a 
    /// LibraryId or the information about the Remoting Types of the Members. This record implicitly specifies that
    /// the Class is in the System Library. This record can be used when the information is deemed unnecessary because 
    /// it is known out of band or can be inferred from context.
    ///
    /// [MS-NRBF] 2.3.2.4 SystemClassWithMembers
    /// </summary>
    internal class SystemClassWithMembers : RecordTypeStructure
    {
        public ClassInfo ClassInfo { get; }

        public SystemClassWithMembers(DeserializeReader reader) : base(reader, RecordType.SystemClassWithMembers)
        {
            ClassInfo = new ClassInfo(reader);
        }

        public override string ToString()
        {
            return $"ClassInfo = {{ {ClassInfo} }}";
        }
    }
}