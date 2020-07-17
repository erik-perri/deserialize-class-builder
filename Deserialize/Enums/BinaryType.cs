namespace DeserializeClassBuilder.Deserialize.Enums
{
    /// <summary>
    /// The BinaryTypeEnumeration identifies the Remoting Type of a Class (2) Member or an Array item. The size of the
    /// enumeration is a BYTE.
    ///
    /// [MS-NRBF] 2.1.2.2 BinaryTypeEnumeration
    /// </summary>
    public enum BinaryType
    {
        Invalid = -1,
        Primitive = 0,
        String = 1,
        Object = 2,
        SystemClass = 3,
        Class = 4,
        ObjectArray = 5,
        StringArray = 6,
        PrimitiveArray = 7,
    };
}