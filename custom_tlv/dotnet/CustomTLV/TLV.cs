using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace CustomTLV;

public static class TLV
{
    private static BinaryFormatter _formatter = new BinaryFormatter();

    public static TLVRecord ReadRecord(NetworkStream stream)
    {
        var typeBuffer = new byte[1];
        stream.Read(typeBuffer, 0, 1);
        var type = typeBuffer[0];
        var lengthBuffer = new byte[2];
        stream.Read(lengthBuffer, 0, 2);
        var length = BitConverter.ToUInt16(lengthBuffer, 0);
        var value = new byte[length];
        stream.Read(value, 0, length);
        // Console.WriteLine($"Read type {type} with length {length} and value {BitConverter.ToString(value)}");
        return new TLVRecord(type, length, value);
    }

    public static void WriteRecord(NetworkStream stream, TLVRecord record)
    {
        var typeBuffer = new byte[1]{ record.Type };
        stream.Write(typeBuffer, 0, 1);
        var lengthBuffer = BitConverter.GetBytes(record.Length);
        stream.Write(lengthBuffer, 0, 2);
        stream.Write(record.Value, 0, record.Length);
        // Console.WriteLine($"Wrote type {record.Type} with length {record.Length} and value {BitConverter.ToString(record.Value)}");
    }

    public static byte[] WriteToByte(TLVRecord record)
    {
        var typeBuffer = new byte[] { record.Type };
        var lengthBuffer = BitConverter.GetBytes(record.Length);
        var value = new byte[1 + 2 + record.Length];

        Array.Copy(typeBuffer, 0, value, 0, 1);
        Array.Copy(lengthBuffer, 0, value, 1, 2);
        Array.Copy(record.Value, 0, value, 3, record.Length);

        return value;
    }

    public static TLVRecord ReadFromByte(byte[] data)
    {
        var type = data[0];
        var length = BitConverter.ToUInt16(data, 1);
        var value = new byte[length];
        Array.Copy(data, 3, value, 0, length);
        return new TLVRecord(type, length, value);
    }

    public static byte[] Encode<T>(T data)
    {
        using var stream = new System.IO.MemoryStream();
        _formatter.Serialize(stream, data);
        return stream.ToArray();
    }

    public static T Decode<T>(byte[] data)
    {
        using var stream = new System.IO.MemoryStream(data);
        return (T)_formatter.Deserialize(stream);
    }
}