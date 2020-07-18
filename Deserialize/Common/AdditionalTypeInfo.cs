using DeserializeClassBuilder.Deserialize.Common.AdditionalInfo;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;

namespace DeserializeClassBuilder.Deserialize.Common
{
    /// <summary>
    ///  Information about the Remoting Type of the Array item in addition to the information provided in the TypeEnum
    ///  field. For the BinaryTypeEnum values of Primitive, SystemClass, Class, or PrimitiveArray, this field contains
    ///  additional information about the The MemberTypeInfo is a common structure that contains type information for
    ///  Class(2) Members.
    ///
    /// [MS-NRBF] 2.3.1.2 MemberTypeInfo
    /// </summary>
    internal partial class AdditionalTypeInfo
    {
        public IAdditionalInfo Info { get; }

        public AdditionalTypeInfo(DeserializeReader reader, BinaryType binaryType)
        {
            switch (binaryType)
            {
                case BinaryType.Primitive:
                    Info = new AdditionalInfoPrimitiveType(reader);
                    break;

                case BinaryType.SystemClass:
                    Info = new AdditionalInfoSystemClass(reader);
                    break;

                case BinaryType.Class:
                    Info = new AdditionalInfoClassTypeInfo(reader);
                    break;

                case BinaryType.PrimitiveArray:
                    Info = new AdditionalInfoPrimitiveType(reader);
                    break;

                case BinaryType.String:
                case BinaryType.Object:
                case BinaryType.ObjectArray:
                case BinaryType.StringArray:
                    break;

                default:
                    throw new InvalidStructureException("Unknown member type");
            }
        }
    }
}