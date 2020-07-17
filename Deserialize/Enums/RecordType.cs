namespace DeserializeClassBuilder.Deserialize.Enums
{
    /// <summary>
    /// This enumeration identifies the type of the record. Each record (except for MemberPrimitiveUnTyped) starts with
    /// a record type enumeration.
    ///
    /// [MS-NRBF] 2.1.2.1 RecordTypeEnumeration
    /// </summary>
    public enum RecordType
    {
        Invalid = -1,
        SerializedStreamHeader = 0,
        ClassWithId = 1,
        SystemClassWithMembers = 2,
        ClassWithMembers = 3,
        SystemClassWithMembersAndTypes = 4,
        ClassWithMembersAndTypes = 5,
        BinaryObjectString = 6,
        BinaryArray = 7,
        MemberPrimitiveTyped = 8,
        MemberReference = 9,
        ObjectNull = 10,
        MessageEnd = 11,
        BinaryLibrary = 12,
        ObjectNullMultiple256 = 13,
        ObjectNullMultiple = 14,
        ArraySinglePrimitive = 15,
        ArraySingleObject = 16,
        ArraySingleString = 17,
        MethodCall = 18,
        MethodReturn = 19,
    };
}