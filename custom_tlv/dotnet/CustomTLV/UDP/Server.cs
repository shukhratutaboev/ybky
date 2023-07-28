using System.Net;
using System.Net.Sockets;
using CustomTLV.Encoders;

namespace CustomTLV.UDP;

public class Server
{
    private readonly IEncoder _encoder;
    private readonly UdpClient _client;

    public Server(string ip, int port)
    {
        _encoder = new JsonEncoder();
        _client = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public async Task StartAsync()
    {
        Console.WriteLine("UDP server started. Listening for incoming connections...");

        while (true)
        {
            var message = await _client.ReceiveAsync();
            Console.WriteLine("Client connected.");

            _ = Task.Run(async () => await HandleClientAsync(message.RemoteEndPoint, message.Buffer));
        }
    }

    public async Task StopAsync()
    {
        _client.Close();
    }

    private async Task HandleClientAsync(IPEndPoint client, byte[] message)
    {
        var record = await TLV.ReadFromByteAsync(message);

        var person = await _encoder.DecodeAsync<Person>(record.Value);

        Console.WriteLine($"Received person: {person.FirstName} {person.LastName} ({person.Age})");

        var response = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = person.Age + 1
        };

        var value = await _encoder.EncodeAsync(response);
        var responseRecord = new TLVRecord(1, (ushort)value.Length, value);

        var responseMessage = await TLV.WriteToByteAsync(responseRecord);

        _client.Send(responseMessage, responseMessage.Length, client);
    }
}