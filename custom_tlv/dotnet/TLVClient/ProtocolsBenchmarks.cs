using System.Net.Http.Json;
using BenchmarkDotNet.Attributes;
using CustomTLV;

namespace TLVClient;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ProtocolsBenchmarks
{
    // private readonly CustomTLV.UDP.Client _udpClient = new();
    // private readonly CustomTLV.TCP.Client _tcpClient = new("127.0.0.2", 8081);
    // private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("http://127.0.0.2:8082") };

    [Benchmark]
    public async Task SendFromUdpAsync()
    {
        var _udpClient = new CustomTLV.UDP.Client();

        await _udpClient.SendAsync(new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 30
        }, "127.0.0.2", 8080);

        _ = await _udpClient.ReceiveAsync<Person>();

        await _udpClient.CloseAsync();
    }

    [Benchmark]
    public async Task SendFromTcpAsync()
    {
        var _tcpClient = new CustomTLV.TCP.Client("127.0.0.2", 8081);

        await _tcpClient.SendAsync(new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 30
        });

        _ = await _tcpClient.ReceiveAsync<Person>();

        await _tcpClient.CloseAsync();
    }

    [Benchmark]
    public async Task SendFromHttpAsync()
    {
        var _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.2:8082")
        };

        var person = new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Age = 30
        };
        var response = await _httpClient.PostAsJsonAsync("api/person", person);
        _ = await response.Content.ReadFromJsonAsync<Person>();

        _httpClient.Dispose();
    }
}