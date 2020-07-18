using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeserializeClassBuilder.Deserialize.Common
{
    /// <summary>
    /// The MemberTypeInfo is a common structure that contains type information for Class(2) Members.
    ///
    /// [MS-NRBF] 2.3.1.2 MemberTypeInfo
    /// </summary>
    internal class MemberTypeInfo
    {
        public ReadOnlyDictionary<string, BinaryType> Types { get; private set; }
        public ReadOnlyDictionary<string, AdditionalTypeInfo> Infos { get; private set; }

        public MemberTypeInfo(DeserializeReader reader, ClassInfo classInfo)
        {
            var types = new Dictionary<string, BinaryType>();
            var infos = new Dictionary<string, AdditionalTypeInfo>();

            foreach (var memberName in classInfo.MemberNames)
            {
                var type = reader.ReadEnum<BinaryType>();
                if (type == BinaryType.Invalid)
                {
                    throw new InvalidStructureException("Invalid class member type");
                }

                types.Add(memberName, type);
            }

            Types = new ReadOnlyDictionary<string, BinaryType>(types);

            foreach (var kvp in types)
            {
                infos.Add(kvp.Key, new AdditionalTypeInfo(reader, kvp.Value));
            }

            Infos = new ReadOnlyDictionary<string, AdditionalTypeInfo>(infos);
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