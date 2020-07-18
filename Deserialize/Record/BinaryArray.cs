using DeserializeClassBuilder.Deserialize.Common;
using DeserializeClassBuilder.Deserialize.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeserializeClassBuilder.Deserialize.Record
{
    /// <summary>
    /// BinaryArray is the most general form of Array records. The record is more verbose than the other Array records.
    ///
    /// [MS-NRBF] 2.4.3.1 BinaryArray
    /// </summary>
    internal class BinaryArray : RecordTypeStructure
    {
        public int ObjectId { get; }
        public BinaryArrayType BinaryArrayType { get; }
        public int Rank { get; }
        public ReadOnlyCollection<int> Lengths { get; }
        public ReadOnlyCollection<int> LowerBounds { get; }
        public BinaryType ItemType { get; }
        public AdditionalTypeInfo AdditionalTypeInfo { get; }
        internal SystemClassWithMembersAndTypes SystemClass { get; }

        public BinaryArray(DeserializeReader reader) : base(reader, RecordType.BinaryArray)
        {
            ObjectId = reader.ReadInt32();
            BinaryArrayType = reader.ReadEnum<BinaryArrayType>();
            Rank = reader.ReadInt32();

            var lengths = new List<int>();
            for (int i = 0; i < Rank; i++)
            {
                lengths.Add(reader.ReadInt32());
            }

            var lowerBounds = new List<int>();
            switch (BinaryArrayType)
            {
                case BinaryArrayType.SingleOffset:
                case BinaryArrayType.JaggedOffset:
                case BinaryArrayType.RectangularOffset:
                    for (int i = 0; i < Rank; i++)
                    {
                        lowerBounds.Add(reader.ReadInt32());
                    }
                    break;

                default:
                    break;
            }

            Lengths = new ReadOnlyCollection<int>(lengths);
            LowerBounds = new ReadOnlyCollection<int>(lowerBounds);
            ItemType = reader.ReadEnum<BinaryType>();
            AdditionalTypeInfo = new AdditionalTypeInfo(reader, ItemType);

            SystemClass = new SystemClassWithMembersAndTypes(reader);
        }

        public override string ToString()
        {
            return $"ObjectId = {ObjectId}; BinaryArrayType = {BinaryArrayType}; Rank = {Rank}; ItemType = {ItemType};" +
                $" SystemClass = {{ {SystemClass} }}";
        }
    }
}