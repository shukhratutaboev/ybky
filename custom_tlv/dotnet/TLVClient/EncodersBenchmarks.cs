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

    [Benchmark]
    public async Task Json()
    {
        var bytes = await _jsonEncoder.EncodeAsync(_person);
        _ = await _jsonEncoder.DecodeAsync<Person>(bytes);
    }

    [Benchmark]
    public async Task Bson()
    {
        var bytes = await _bsonEncoder.EncodeAsync(_person);
        _ = await _bsonEncoder.DecodeAsync<Person>(bytes);
    }

    [Benchmark]
    public async Task BF()
    {
        var bytes = await _bfEncoder.EncodeAsync(_person);
        _ = await _bfEncoder.DecodeAsync<Person>(bytes);
    }

    [Benchmark]
    public async Task ProtoBuf()
    {
        var person = new MyClass()
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 30
        };
        var bytes = await _protoBufEncoder.EncodeAsync(person);
        var p = await _protoBufEncoder.DecodeAsync<MyClass>(bytes);
        _ = new Person
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            Age = p.Age
        };
    }
}