using Google.Protobuf;

namespace CustomTLV.Encoders;

public class ProtoBufEncoder : IEncoder
{
    public async Task<byte[]> EncodeAsync<T>(T data)
    {
        if (data is Person person)
        {
            var p = new MyClass
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = person.Age
            };
            using var ms = new MemoryStream();
            p.WriteTo(ms);
            return ms.ToArray();
        }

        throw new ArgumentException("Invalid type");
    }

    public async Task<T> DecodeAsync<T>(byte[] data)
    {
        if (typeof(T) == typeof(Person))
        {
            using var ms = new MemoryStream(data);
            var p = MyClass.Parser.ParseFrom(ms);
            return (T)(object)new Person
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age
            };
        }

        throw new ArgumentException("Invalid type");
    }

    public Task<byte[]> EncodeAsync<T>(T data, byte[] key = null, byte[] publicKey = null)
    {
        if (data is Person person)
        {
            var p = new MyClass
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Age = person.Age
            };
            using var ms = new MemoryStream();
            p.WriteTo(ms);
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

        throw new ArgumentException("Invalid type");
    }

    public Task<T> DecodeAsync<T>(byte[] data, byte[] key = null, byte[] privateKey = null)
    {
        if (typeof(T) == typeof(Person))
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
            var p = MyClass.Parser.ParseFrom(ms);
            return Task.FromResult((T)(object)new Person
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age
            });
        }

        throw new ArgumentException("Invalid type");
    }
}