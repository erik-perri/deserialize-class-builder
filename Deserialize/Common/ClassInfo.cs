using DeserializeClassBuilder.Deserialize.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeserializeClassBuilder.Deserialize.Common
{
    /// <summary>
    /// ClassInfo is a common structure used by all the Class (2) records.
    ///
    /// [MS-NRBF] 2.3.1.1 ClassInfo
    /// </summary>
    public class ClassInfo
    {
        public int ObjectId { get; private set; }
        public string ClassName { get; private set; }
        public int MemberCount { get; private set; }
        public ReadOnlyCollection<string> MemberNames { get; private set; }

        public ClassInfo(DeserializeReader reader)
        {
            ObjectId = reader.ReadInt32();
            ClassName = reader.ReadString();
            if (string.IsNullOrWhiteSpace(ClassName))
            {
                throw new InvalidStructureException("Invalid class name");
            }

            MemberCount = reader.ReadInt32();
            if (MemberCount < 0 || MemberCount > 10240)
            {
                throw new InvalidStructureException($"Non-sane member count {MemberCount}");
            }

            var memberNames = new List<string>();
            for (var i = 0; i < MemberCount; i++)
            {
                memberNames.Add(reader.ReadString());
            }

            MemberNames = new ReadOnlyCollection<string>(memberNames);
        }

        public override string ToString()
        {
            return $"ObjectId = {ObjectId}; MemberCount = {MemberCount}; ClassName = {ClassName};" +
                $" MemberNames = {string.Join(",", MemberNames)}";
        }
    }
}