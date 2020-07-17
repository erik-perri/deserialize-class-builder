using DeserializeClassBuilder.Deserialize;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Record;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeserializeClassBuilder
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: builder <file> <class-name-substr>");
                return 1;
            }

            var fileBytes = File.ReadAllBytes(args[0]);
            var searchBytes = System.Text.Encoding.UTF8.GetBytes(args[1]);

            using var fileStream = new MemoryStream(fileBytes);
            using var reader = new DeserializeReader(fileStream);

            var header = new SerializationHeaderRecord(reader);
            var library = new BinaryLibrary(reader);
            var records = new List<ClassWithMemberAndTypes>();

            var classNamePositions = BoyerMoore.IndexesOf(fileBytes, searchBytes);

            foreach (var classNamePosition in classNamePositions)
            {
                var updatedPosition = classNamePosition;
                updatedPosition -= 1; // Name size
                updatedPosition -= 4; // Object id
                updatedPosition -= 1; // RecordType (ClassWithMembersAndTypes)

                if (updatedPosition < 0)
                {
                    continue;
                }

                reader.BaseStream.Position = updatedPosition;

                var type = reader.PeekEnum<RecordType>();

                if (type != RecordType.ClassWithMembersAndTypes)
                {
                    continue;
                }

                try
                {
                    var record = new ClassWithMemberAndTypes(reader);
                    records.Add(record);

                    Console.WriteLine($"Parsed {record.ClassInfo.ClassName} at {updatedPosition}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse at {updatedPosition} {e.Message}");
                }
            }

            Directory.CreateDirectory("output");

            foreach(var record in records)
            {
                var classTemplate = "using System;\r\n" +
                    "\r\n" +
                    "namespace DeserializeTesting.Deserialize.Lo.Tables\r\n" +
                    "{\r\n" +
                    "    [Serializable]\r\n" +
                    $"    public class {record.ClassInfo.ClassName} : ITable\r\n" +
                    "    {\r\n";

                foreach (var memberName in record.ClassInfo.MemberNames)
                {
                    classTemplate += $"        // {GetMemberElement(memberName, record.MemberTypeInfo.Types[memberName], record.MemberTypeInfo.Infos[memberName])}\r\n";
                }
                
                classTemplate +=
                    "     }\r\n" +
                    "}\r\n";

                File.WriteAllText($"output\\{record.ClassInfo.ClassName}.cs", classTemplate);
            }

            return 0;
        }

        public static string GetMemberElement(string memberName, BinaryType binaryType, Deserialize.Common.AdditionalTypeInfo additionalTypeInfo)
        {
            switch(binaryType)
            {
                case BinaryType.String:
                    {
                        return $"public string {GetMemberName(memberName)}";
                    }
                case BinaryType.Primitive:
                    {
                        if (!(additionalTypeInfo.Info is Deserialize.Common.AdditionalTypeInfo.AdditionalInfoPrimitiveType primitiveType))
                        {
                            throw new Exception("Unexpected additional type");
                        }

                        switch(primitiveType.Type)
                        {
                            case PrimitiveType.Int32:
                                return $"public int {GetMemberName(memberName)}";

                            default:
                                return $"public {primitiveType.Type} {memberName}";
                        }
                    }
                case BinaryType.SystemClass:
                    {
                        if (!(additionalTypeInfo.Info is Deserialize.Common.AdditionalTypeInfo.AdditionalInfoSystemClass systemClass))
                        {
                            throw new Exception("Unexpected additional type");
                        }

                        var dictString = "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[";
                        if (systemClass.ClassName.StartsWith(dictString, StringComparison.Ordinal))
                        {
                            var refClass = $"{systemClass.ClassName.Substring(dictString.Length).Split(",").First()}";

                            return $"public Dictionary<string, {refClass}> {memberName};";
                        }

                        return $"{memberName} {systemClass.ClassName}";
                    }
                default:
                    Console.WriteLine("Unhandled");
                    return $"{memberName}";
            }

            return null;
        }

        public static string GetMemberName(string memberName)
        {
            var match = Regex.Match(memberName, "^<(.*)>k__BackingField$");
            if (!match.Success)
            {
                return $"{memberName};";
            }

            return $"{match.Groups[1]} {{ get; set; }}";
        }
        /*
        private static void ExamineForDeserialization(Stream file)
        {
            using var reader = new DeserializationReader(file);

            var header = new SerializationHeaderRecord(reader);
            var library = new BinaryLibrary(reader);

            var records = new List<RecordTypeStructure>();
            var references = new List<DeserializeStructure.Record.MemberReference>();

            var searching = true;
            while (searching)
            {
                var nextType = reader.PreviewEnum<RecordType>();
                var position = reader.BaseStream.Position;

                DebugOutput.WriteLine($"Found {nextType} at {position}");

                switch (nextType)
                {
                    case RecordType.ClassWithMembersAndTypes:
                        {
                            var record = new ClassWithMemberAndTypes(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");

                            // The class should be followed by a reference for each member
                            foreach (var kvp in record.MemberTypeInfo.Types)
                            {
                                switch (kvp.Value)
                                {
                                    case BinaryType.Primitive:
                                        var t = reader.ReadEnum<RecordType>();
                                        DebugOutput.WriteLine($"Unsaved primitive type for {kvp.Key}");
                                        break;

                                    case BinaryType.String:
                                    case BinaryType.SystemClass:
                                    case BinaryType.Class:
                                        references.Add(new DeserializeStructure.Record.MemberReference(reader));
                                        break;

                                    default:
                                        DebugOutput.WriteLine($"Unexpected method type in {record}");
                                        searching = false;
                                        break;
                                }
                            }
                        }
                        break;

                    case RecordType.ClassWithMembers:
                        {
                            var record = new ClassWithMembers(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.ClassWithId:
                        {
                            var record = new ClassWithId(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.SystemClassWithMembersAndTypes:
                        {
                            var record = new SystemClassWithMembersAndTypes(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");

                            // The class should be followed by a reference for each member
                            foreach (var kvp in record.MemberTypeInfo.Types)
                            {
                                switch (kvp.Value)
                                {
                                    case BinaryType.Primitive:
                                        var unknown = reader.ReadInt32();
                                        DebugOutput.WriteLine($"Unsaved primitive value {unknown} for {kvp.Key} at {reader.BaseStream.Position}");
                                        break;

                                    case BinaryType.String:
                                    case BinaryType.SystemClass:
                                    case BinaryType.Class:
                                        references.Add(new DeserializeStructure.Record.MemberReference(reader));
                                        break;

                                    default:
                                        DebugOutput.WriteLine($"Unexpected method type at {reader.BaseStream.Position}");
                                        searching = false;
                                        break;
                                }
                            }
                        }
                        break;

                    case RecordType.SystemClassWithMembers:
                        {
                            var record = new SystemClassWithMembers(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.BinaryArray:
                        {
                            var record = new BinaryArray(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.BinaryObjectString:
                        {
                            var record = new BinaryObjectString(reader);
                            records.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.MemberReference:
                        {
                            var record = new DeserializeStructure.Record.MemberReference(reader);
                            references.Add(record);

                            DebugOutput.WriteLine($"Parsed {record.GetType().Name} at {position} {record}");
                        }
                        break;

                    case RecordType.Invalid:
                        {
                            DebugOutput.WriteLine($"Unexpected type at {position}");
                            searching = false;
                        }
                        break;

                    default:
                        {
                            DebugOutput.WriteLine($"Unhandled type {nextType} at {reader.BaseStream.Position}");
                            searching = false;
                        }
                        break;
                }
            }
        }
        */
    }
}
