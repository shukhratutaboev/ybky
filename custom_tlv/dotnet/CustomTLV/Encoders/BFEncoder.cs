using System.Runtime.Serialization.Formatters.Binary;

namespace CustomTLV.Encoders
{
    public class BFEncoder : IEncoder
    {
        private readonly BinaryFormatter _formatter = new();

        public async Task<byte[]> EncodeAsync<T>(T data)
        {
            using var stream = new MemoryStream();
            _formatter.Serialize(stream, data);
            return stream.ToArray();
        }

        public async Task<T> DecodeAsync<T>(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return (T)_formatter.Deserialize(stream);
        }
    }

    public interface IEncoder
    {
        Task<byte[]> EncodeAsync<T>(T data);
        Task<T> DecodeAsync<T>(byte[] data);
    }
}