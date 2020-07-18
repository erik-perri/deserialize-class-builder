using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Common.AdditionalInfo
{
    internal class AdditionalInfoPrimitiveType : IAdditionalInfo
    {
        public PrimitiveType Type { get; private set; }

        public AdditionalInfoPrimitiveType(DeserializeReader reader)
        {
            Type = reader.ReadEnum<PrimitiveType>();
            if (Type == PrimitiveType.Invalid)
            {
                throw new InvalidStructureException("Invalid class member type");
            }
        }

        public override string ToString()
        {
            return $"Type = {Type}";
        }
    }
}