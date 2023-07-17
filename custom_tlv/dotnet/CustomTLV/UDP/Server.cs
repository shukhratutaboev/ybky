using System.Net;
using System.Net.Sockets;

namespace CustomTLV.UDP;

public class Server
{
    private readonly UdpClient _client;

    public Server(string ip, int port)
    {
        _client = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public void Start()
    {
        Console.WriteLine("Server started. Listening for incoming connections...");

        while (true)
        {
            var client = new IPEndPoint(IPAddress.Any, 0);
            var message = _client.Receive(ref client);
            Console.WriteLine("Client connected.");

            Task.Run(() => HandleClient(client, message));
        }
    }

    public void Stop()
    {
        _client.Close();
    }

    private void HandleClient(IPEndPoint client, byte[] message)
    {
        var record = TLV.ReadFromByte(message);

        var person = TLV.Decode<Person>(record.Value);

        Console.WriteLine($"Received person: {person.FirstName} {person.LastName} ({person.Age})");

        var response = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = person.Age + 1
        };

        var value = TLV.Encode(response);
        var responseRecord = new TLVRecord(1, (ushort)value.Length, value);

        var responseMessage = TLV.WriteToByte(responseRecord);

        _client.Send(responseMessage, responseMessage.Length, client);
    }
}