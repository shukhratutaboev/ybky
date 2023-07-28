using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace CustomTLV;

public static class TLV
{
    public static async Task<TLVRecord> ReadRecordAsync(NetworkStream stream)
    {
        var typeBuffer = new byte[1];
        _ = await stream.ReadAsync(typeBuffer.AsMemory(0, 1));
        var type = typeBuffer[0];
        var lengthBuffer = new byte[2];
        _ = await stream.ReadAsync(lengthBuffer.AsMemory(0, 2));
        var length = BitConverter.ToUInt16(lengthBuffer, 0);
        Console.WriteLine($"Type: {type}, Length: {length}");
        var value = new byte[length];
        _ = await stream.ReadAsync(value.AsMemory(0, length));
        return new TLVRecord(type, length, value);
    }

    public static async Task WriteRecordAsync(NetworkStream stream, TLVRecord record)
    {
        var typeBuffer = new byte[1]{ record.Type };
        await stream.WriteAsync(typeBuffer.AsMemory(0, 1));
        var lengthBuffer = BitConverter.GetBytes(record.Length);
        await stream.WriteAsync(lengthBuffer.AsMemory(0, 2));
        await stream.WriteAsync(record.Value.AsMemory(0, record.Length));
    }

    public static async Task<byte[]> WriteToByteAsync(TLVRecord record)
    {
        var typeBuffer = new byte[] { record.Type };
        var lengthBuffer = BitConverter.GetBytes(record.Length);
        var value = new byte[1 + 2 + record.Length];

        Array.Copy(typeBuffer, 0, value, 0, 1);
        Array.Copy(lengthBuffer, 0, value, 1, 2);
        Array.Copy(record.Value, 0, value, 3, record.Length);

        return value;
    }

    public static async Task<TLVRecord> ReadFromByteAsync(byte[] data)
    {
        var type = data[0];
        var length = BitConverter.ToUInt16(data, 1);
        var value = new byte[length];
        Array.Copy(data, 3, value, 0, length);
        return new TLVRecord(type, length, value);
    }
}