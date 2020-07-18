using System;
using System.IO;
using System.Text;

namespace DeserializeClassBuilder.Deserialize
{
    internal class DeserializeReader : BinaryReader
    {
        public DeserializeReader(Stream input) : base(input)
        {
        }

        public DeserializeReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public DeserializeReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public TEnum ReadEnum<TEnum>() where TEnum : struct
        {
            var typeAsByte = ReadByte();
            if (Enum.TryParse($"{typeAsByte}", out TEnum result))
            {
                return result;
            }

            return default;
        }

        public TEnum PeekEnum<TEnum>() where TEnum : struct
        {
            var type = ReadEnum<TEnum>();

            BaseStream.Position--;

            return type;
        }
    }
}
