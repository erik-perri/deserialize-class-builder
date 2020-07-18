using DeserializeClassBuilder.Deserialize.Enums;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The BinaryLibrary record associates an INT32 ID (as specified in [MS-DTYP] section 2.2.22) with a Library name.
    /// This allows other records to reference the Library name by using the ID.This approach reduces the wire size
    /// when there are multiple records that reference the same Library name.
    ///
    /// [MS-NRBF] 2.6.2 BinaryLibrary
    /// </summary>
    internal class BinaryLibrary : RecordTypeStructure
    {
        public int LibraryId { get; }
        public string LibraryName { get; }

        public BinaryLibrary(DeserializeReader reader) : base(reader, RecordType.BinaryLibrary)
        {
            LibraryId = reader.ReadInt32();
            LibraryName = reader.ReadString();
        }

        public override string ToString()
        {
            return $"LibraryId = {LibraryId}; LibraryName = {LibraryName}";
        }
    }
}