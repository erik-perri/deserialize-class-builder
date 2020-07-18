namespace DeserializeClassBuilder.Deserialize.Enums
{
    /// <summary>
    /// The BinaryArrayTypeEnumeration is used to denote the type of an Array. The size of the enumeration is 1 byte.
    /// It is used by the Array records.
    ///
    /// [MS-NRBF] 2.4.1.1 BinaryArrayTypeEnumeration
    /// </summary>
    internal enum BinaryArrayType
    {
        Invalid = -1,
        Single = 0,
        Jagged = 1,
        Rectangular = 2,
        SingleOffset = 3,
        JaggedOffset = 4,
        RectangularOffset = 5,
    };
}