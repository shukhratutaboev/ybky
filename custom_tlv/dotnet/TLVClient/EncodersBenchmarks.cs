using BenchmarkDotNet.Attributes;
using CustomTLV;
using CustomTLV.Encoders;

namespace TLVClient;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EncodersBenchmarks
{
    private Person _person = new Person
    {
        FirstName = "Jane",
        LastName = "Doe",
        Age = 30
    };

    private JsonEncoder _jsonEncoder = new();
    private BsonEncoder _bsonEncoder = new();
    private BFEncoder _bfEncoder = new();
    private ProtoBufEncoder _protoBufEncoder = new();
    private byte[] _key = SymmetricEncryptor.GenerateKey();
    private (byte[] PrivateKey, byte[] PublicKey) _keys = AssymentricEncryptor.GenerateKeys();

    [Benchmark]
    public async Task Json()
    {
        var bytes = await _jsonEncoder.EncodeAsync(_person);
        var p = await _jsonEncoder.DecodeAsync<Person>(bytes);
        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task Bson()
    {
        var bytes = await _bsonEncoder.EncodeAsync(_person);
        var p = await _bsonEncoder.DecodeAsync<Person>(bytes);
        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task BF()
    {
        var bytes = await _bfEncoder.EncodeAsync(_person);
        var p = await _bfEncoder.DecodeAsync<Person>(bytes);
        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task ProtoBuf()
    {
        var bytes = await _protoBufEncoder.EncodeAsync(_person);
        var p = await _protoBufEncoder.DecodeAsync<Person>(bytes);
        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task JsonWithKey()
    {
        var bytes = await _jsonEncoder.EncodeAsync(_person, _key);
        var p = await _jsonEncoder.DecodeAsync<Person>(bytes, _key);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task BsonWithKey()
    {
        var bytes = await _bsonEncoder.EncodeAsync(_person, _key);
        var p = await _bsonEncoder.DecodeAsync<Person>(bytes, _key);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task BFWithKey()
    {
        var bytes = await _bfEncoder.EncodeAsync(_person, _key);
        var p = await _bfEncoder.DecodeAsync<Person>(bytes, _key);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task ProtoBufWithKey()
    {
        var bytes = await _protoBufEncoder.EncodeAsync(_person, _key);
        var p = await _protoBufEncoder.DecodeAsync<Person>(bytes, _key);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task JsonWithPublicKey()
    {
        var bytes = await _jsonEncoder.EncodeAsync(_person, publicKey: _keys.PublicKey);
        var p = await _jsonEncoder.DecodeAsync<Person>(bytes, privateKey: _keys.PrivateKey);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task BsonWithPublicKey()
    {
        var bytes = await _bsonEncoder.EncodeAsync(_person, publicKey: _keys.PublicKey);
        var p = await _bsonEncoder.DecodeAsync<Person>(bytes, privateKey: _keys.PrivateKey);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task BFWithPublicKey()
    {
        var bytes = await _bfEncoder.EncodeAsync(_person, publicKey: _keys.PublicKey);
        var p = await _bfEncoder.DecodeAsync<Person>(bytes, privateKey: _keys.PrivateKey);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    [Benchmark]
    public async Task ProtoBufWithPublicKey()
    {
        var bytes = await _protoBufEncoder.EncodeAsync(_person, publicKey: _keys.PublicKey);
        var p = await _protoBufEncoder.DecodeAsync<Person>(bytes, privateKey: _keys.PrivateKey);

        if (!CheckPerson(p))
        {
            throw new Exception("Invalid person");
        }
    }

    public bool CheckPerson(Person person)
    {
        return person.FirstName == _person.FirstName &&
            person.LastName == _person.LastName &&
            person.Age == _person.Age;
    }
}