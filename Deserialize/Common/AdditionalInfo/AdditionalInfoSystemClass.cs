namespace DeserializeClassBuilder.Deserialize.Common.AdditionalInfo
{
    internal class AdditionalInfoSystemClass : IAdditionalInfo
    {
        public string ClassName { get; private set; }

        public AdditionalInfoSystemClass(DeserializeReader reader)
        {
            ClassName = reader.ReadString();
        }

        public override string ToString()
        {
            return $"ClassName = {ClassName}";
        }
    }
}