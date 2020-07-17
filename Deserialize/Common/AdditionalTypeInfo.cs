using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;
using DeserializeClassBuilder.Deserialize.Record;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    public class AdditionalTypeInfo
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

        public interface IAdditionalInfo
        {
        }

        public class AdditionalInfoPrimitiveType : IAdditionalInfo
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

        public class AdditionalInfoSystemClass : IAdditionalInfo
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

        public class AdditionalInfoClassTypeInfo : IAdditionalInfo
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
}