using DeserializeClassBuilder.Deserialize.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DeserializeClassBuilder.Deserialize.Common
{
    /// <summary>
    /// ClassInfo is a common structure used by all the Class (2) records.
    ///
    /// [MS-NRBF] 2.3.1.1 ClassInfo
    /// </summary>
    internal class ClassInfo
    {
        public int ObjectId { get; private set; }
        public string ClassName { get; private set; }
        public int MemberCount { get; private set; }
        public ReadOnlyCollection<string> MemberNames { get; private set; }

        public ClassInfo(DeserializeReader reader)
        {
            ObjectId = reader.ReadInt32();

            // TODO Find class name length limit or a better way to detect failure
            var classNameLength = Peek7BitEncodedInt(reader);
            if (classNameLength < 1 || classNameLength > 256)
            {
                throw new InvalidStructureException($"Invalid class name length {classNameLength}");
            }

            ClassName = reader.ReadString();

            var classNameAsBytes = Encoding.UTF8.GetBytes(ClassName);
            // Make sure the class name only contains bytes between space and tilde, I assume anything outside this is
            // invalid for class names but cannot find any documentation about allowed characters in MS-NRBF/MS-NRTP.
            if (classNameAsBytes.Any((b) => b < System.Convert.ToByte(' ') || b > System.Convert.ToByte('~')))
            {
                throw new InvalidStructureException($"Invalid class name characters");
            }

            if (string.IsNullOrWhiteSpace(ClassName) || ClassName.Length > 256)
            {
                throw new InvalidStructureException($"Invalid class name");
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

        /// <summary>
        /// Copied from mscorlib reference. Copyright Microsoft Corporation.
        /// https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs,f30b8b6e8ca06e0f
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected internal int Peek7BitEncodedInt(DeserializeReader reader)
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int readBytes = 0;
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new InvalidStructureException("Invalid name size");

                // ReadByte handles end of stream cases for us.
                b = reader.ReadByte();
                readBytes++;
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            while (readBytes-- > 0)
            {
                reader.BaseStream.Position--;
            }

            return count;
        }
    }
}