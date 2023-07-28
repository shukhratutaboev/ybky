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
}