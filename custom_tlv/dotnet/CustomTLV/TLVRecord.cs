namespace CustomTLV;
public struct TLVRecord
{
    public TLVRecord(byte type, ushort length, byte[] value)
    {
        Type = type;
        Length = length;
        Value = value;
    }

    public byte Type { get; set; }
    public ushort Length { get; set; }
    public byte[] Value { get; set; }
}