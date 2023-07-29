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

    public Task<byte[]> EncodeAsync<T>(T data, byte[] key = null, byte[] publicKey = null)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);

        if (key != null)
        {
            return Task.FromResult(SymmetricEncryptor.Encrypt(bytes, key));
        }
        else if (publicKey != null)
        {
            return Task.FromResult(AssymentricEncryptor.Encrypt(bytes, publicKey));
        }

        return Task.FromResult(bytes);
    }

    public Task<T> DecodeAsync<T>(byte[] data, byte[] key = null, byte[] privateKey = null)
    {
        var bytes = data;

        if (key != null)
        {
            bytes = SymmetricEncryptor.Decrypt(bytes, key);
        }
        else if (privateKey != null)
        {
            bytes = AssymentricEncryptor.Decrypt(bytes, privateKey);
        }

        var json = Encoding.UTF8.GetString(bytes);
        return Task.FromResult(JsonSerializer.Deserialize<T>(json));
    }
}