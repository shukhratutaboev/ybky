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

        public Task<byte[]> EncodeAsync<T>(T data, byte[] key = null, byte[] publicKey = null)
        {
            using var stream = new MemoryStream();
            _formatter.Serialize(stream, data);
            var bytes = stream.ToArray();

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

            using var stream = new MemoryStream(bytes);
            return Task.FromResult((T)_formatter.Deserialize(stream));
        }
    }

    public interface IEncoder
    {
        Task<byte[]> EncodeAsync<T>(T data);
        Task<byte[]> EncodeAsync<T>(T data, byte[] key = null, byte[] publicKey = null);
        Task<T> DecodeAsync<T>(byte[] data);
        Task<T> DecodeAsync<T>(byte[] data, byte[] key = null, byte[] privateKey = null);
    }
}