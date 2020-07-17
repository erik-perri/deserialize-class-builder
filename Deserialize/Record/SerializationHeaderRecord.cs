using DeserializeClassBuilder.Deserialize.Enums;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// The SerializationHeaderRecord record MUST be the first record in a binary serialization. This record has the
    /// major and minor version of the format and the IDs of the top object and the headers.
    ///
    /// [MS-NRBF] 2.6.1 SerializationHeaderRecord
    /// </summary>
    public class SerializationHeaderRecord : RecordTypeStructure
    {
        public int RootId { get; }
        public int HeaderId { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }

        public SerializationHeaderRecord(DeserializeReader reader) : base(reader, RecordType.SerializedStreamHeader)
        {
            RootId = reader.ReadInt32();
            HeaderId = reader.ReadInt32();
            MajorVersion = reader.ReadInt32();
            MinorVersion = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"RootId = {RootId}; HeaderId = {HeaderId}; MajorVersion = {MajorVersion};" +
                $" MinorVersion = {MinorVersion}";
        }
    }
}