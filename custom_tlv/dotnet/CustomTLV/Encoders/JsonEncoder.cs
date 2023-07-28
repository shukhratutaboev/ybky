using System.Text;
using System.Text.Json;

namespace CustomTLV.Encoders;

public class JsonEncoder : IEncoder
{
    public async Task<byte[]> EncodeAsync<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }

    public async Task<T> DecodeAsync<T>(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }
}