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
}