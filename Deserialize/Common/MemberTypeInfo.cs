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
    }
}