using DeserializeClassBuilder.Deserialize.Common.AdditionalInfo;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Record;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeserializeClassBuilder
{
    internal class ClassStructureFactory
    {
        public static ClassStructure BuildFromClassWithMemberAndTypes(ClassWithMemberAndTypes record)
        {
            var structure = new ClassStructure();

            var dotPosition = record.ClassInfo.ClassName.IndexOf('.');
            if (dotPosition != -1)
            {
                structure.Namespace = record.ClassInfo.ClassName.Substring(0, dotPosition);
                structure.Name = record.ClassInfo.ClassName.Substring(dotPosition + 1);
            }
            else
            {
                structure.Name = record.ClassInfo.ClassName;
            }

            foreach (var memberName in record.ClassInfo.MemberNames)
            {
                var type = record.MemberTypeInfo.Types[memberName];
                var info = record.MemberTypeInfo.Infos[memberName];

                var parsedMemberName = memberName;
                // The member name shows up as Class+MemberName if it exists as a subclass and the parent contains the
                // member.
                var splitPosition = parsedMemberName.IndexOf('+');
                if (splitPosition != -1)
                {
                    var parentClassName = parsedMemberName.Substring(0, splitPosition);
                    parsedMemberName = parsedMemberName.Substring(splitPosition + 1);

                    if (structure.Extends == null)
                    {
                        structure.Extends = new ClassStructure() { Name = parentClassName };
                    }
                }

                var isProperty = false;
                var match = Regex.Match(parsedMemberName, "<([^>]+)>k__BackingField");
                if (match.Success)
                {
                    isProperty = true;
                    parsedMemberName = match.Groups[1].Value;
                }

                var memberStructure = new ClassStructure.MemberStructure()
                {
                    Name = parsedMemberName,
                    IsProperty = isProperty,
                };

                switch (type)
                {
                    case BinaryType.Primitive:
                        memberStructure.Type = ConvertType((info.Info as AdditionalInfoPrimitiveType).Type);
                        break;

                    case BinaryType.String:
                        memberStructure.Type = "string";
                        break;

                    case BinaryType.Class:
                        memberStructure.Type = (info.Info as AdditionalInfoClassTypeInfo).ClassName;
                        break;

                    case BinaryType.SystemClass:
                        memberStructure.Type = GetTypeString((info.Info as AdditionalInfoSystemClass).ClassName);
                        break;

                    default:
                        memberStructure.Type = $"// {type}";
#if DEBUG
                        Debugger.Break();
#endif
                        break;
                }

                if (memberStructure != null)
                {
                    // If we inherited from a parent we need to add the member to it
                    if (splitPosition != -1)
                    {
                        structure.Extends.Members.Add(memberStructure);
                    }
                    else
                    {
                        structure.Members.Add(memberStructure);
                    }
                }
            }

            return structure;
        }

        private static string GetTypeString(string typeString)
        {
            typeString = new Regex(@", Assembly-CSharp").Replace(typeString, "");
            typeString = new Regex(@", mscorlib").Replace(typeString, "");
            typeString = new Regex(@", Version=[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+").Replace(typeString, "");
            typeString = new Regex(@", Culture=[a-z]+").Replace(typeString, "");
            typeString = new Regex(@", PublicKeyToken=[0-9a-z]+").Replace(typeString, "");

            var graveChar = '`';
            var gravePosition = typeString.IndexOf(graveChar);
            if (gravePosition == -1)
            {
                return typeString;
            }

            var memberType = typeString.Substring(0, gravePosition);

            var genericTypesString = typeString.Substring(gravePosition + 1);
            var typeStartPosition = genericTypesString.IndexOf('[');
            if (typeStartPosition == -1)
            {
                throw new InvalidOperationException("Invalid type string");
            }

            var genericTypeCount = int.Parse(genericTypesString.Substring(0, typeStartPosition));

            genericTypesString = genericTypesString.Substring(typeStartPosition);
            genericTypesString = genericTypesString.Substring(2, genericTypesString.Length - 4);

            var types = genericTypesString.Split("],[");
            if (types.Length != genericTypeCount)
            {
                throw new InvalidOperationException("Invalid type count");
            }

            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].IndexOf(graveChar) != -1)
                {
                    types[i] = GetTypeString(types[i]);
                }
            }

            return $"{memberType}<{string.Join(", ", types)}>";
        }

        private static string ConvertType(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Boolean:
                    return "bool";

                case PrimitiveType.Byte:
                case PrimitiveType.Char:
                case PrimitiveType.Decimal:
                case PrimitiveType.Double:
                case PrimitiveType.Null:
                case PrimitiveType.String:
                    return type.ToString().ToLowerInvariant();

                case PrimitiveType.Int16:
                case PrimitiveType.Int32:
                case PrimitiveType.Int64:
                case PrimitiveType.SByte:
                case PrimitiveType.Single:
                case PrimitiveType.TimeSpan:
                case PrimitiveType.DateTime:
                case PrimitiveType.UInt16:
                case PrimitiveType.UInt32:
                case PrimitiveType.UInt64:
                    return type.ToString();

                case PrimitiveType.UNUSED:
                    return "UNUSED";

                case PrimitiveType.Invalid:
                default:
                    return "INVALID";
            }
        }
    }
}