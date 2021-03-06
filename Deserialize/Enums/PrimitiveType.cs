﻿namespace DeserializeClassBuilder.Deserialize.Enums
{
    /// <summary>
    /// The PrimitiveTypeEnumeration identifies a Primitive Type value. The size of the enumeration is a BYTE.
    ///
    /// [MS-NRBF] 2.1.2.3 PrimitiveTypeEnumeration
    /// </summary>
    internal enum PrimitiveType
    {
        Invalid = -1,
        Boolean = 1,
        Byte = 2,
        Char = 3,
        UNUSED = 4,
        Decimal = 5,
        Double = 6,
        Int16 = 7,
        Int32 = 8,
        Int64 = 9,
        SByte = 10,
        Single = 11,
        TimeSpan = 12,
        DateTime = 13,
        UInt16 = 14,
        UInt32 = 15,
        UInt64 = 16,
        Null = 17,
        String = 18,
    };
}