using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace CustomTLV.Encoders;

public class BsonEncoder : IEncoder
{
    public async Task<byte[]> EncodeAsync<T>(T data)
    {
        using var ms = new MemoryStream();
        using var writer = new BsonBinaryWriter(ms);
        BsonSerializer.Serialize(writer, data);

        return ms.ToArray();
    }

    public async Task<T> DecodeAsync<T>(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BsonBinaryReader(ms);

        return BsonSerializer.Deserialize<T>(reader);
    }

    public Task<byte[]> EncodeAsync<T>(T data, byte[] key = null, byte[] publicKey = null)
    {
        using var ms = new MemoryStream();
        using var writer = new BsonBinaryWriter(ms);
        BsonSerializer.Serialize(writer, data);

        var bytes = ms.ToArray();

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

        using var ms = new MemoryStream(bytes);
        using var reader = new BsonBinaryReader(ms);

        return Task.FromResult(BsonSerializer.Deserialize<T>(reader));
    }
}