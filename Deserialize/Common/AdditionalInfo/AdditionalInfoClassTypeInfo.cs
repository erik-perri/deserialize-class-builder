namespace DeserializeClassBuilder.Deserialize.Common.AdditionalInfo
{
    internal class AdditionalInfoClassTypeInfo : IAdditionalInfo
    {
        public string ClassName { get; private set; }
        public int LibraryId { get; private set; }

        public AdditionalInfoClassTypeInfo(DeserializeReader reader)
        {
            ClassName = reader.ReadString();
            LibraryId = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"ClassName = {ClassName}; LibraryId = {LibraryId}";
        }
    }
}