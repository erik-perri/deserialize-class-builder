using System.Collections.Generic;
using System.IO;

namespace DeserializeClassBuilder
{
    internal class ClassStructure
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public ClassStructure Extends { get; set; }

        public List<MemberStructure> Members { get; set; } = new List<MemberStructure>();

        public class MemberStructure
        {
            public string Type { get; set; }

            public string Name { get; set; }

            public bool IsProperty { get; set; }

            public override string ToString()
            {
                return $"{Type} {Name} {IsProperty}";
            }
        }

        public void WriteTo(string classFilePath, string namespaceToUse)
        {
            var extendsSuffix = Extends == null ? "" : $" : {Extends.Name}";
            var propertySuffix = " { get; set; }";

            var namespaceString = $"{namespaceToUse}";
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                namespaceString += $".{Namespace}";
            }

            var classTemplate = "using System;\r\n" +
                "\r\n" +
                $"namespace {namespaceString}\r\n" +
                "{\r\n" +
                "    [Serializable]\r\n" +
                $"    public class {Name}{extendsSuffix}\r\n" +
                "    {\r\n";

            foreach (var member in Members)
            {
                classTemplate += $"        public {member.Type} {member.Name}{(member.IsProperty ? propertySuffix : ";")}\r\n";
            }

            classTemplate +=
                "     }\r\n" +
                "}\r\n";

            File.WriteAllText(classFilePath, classTemplate);
        }

        public override string ToString()
        {
            if (Extends == null)
            {
                return $"{Name}";
            }

            return $"{Name} : {Extends.Name}";
        }
    }
}