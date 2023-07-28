using System.Net;
using System.Net.Sockets;
using CustomTLV.Encoders;

namespace CustomTLV.TCP;

public class Server
{
    private readonly IEncoder _encoder;
    private readonly TcpListener _listener;

    public Server(string ip, int port)
    {
        _encoder = new ProtoBufEncoder();
        _listener = new TcpListener(IPAddress.Parse(ip), port);
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("TCP server started. Listening for incoming connections...");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            _ = Task.Run(async () => await HandleClientAsync(client));
        }
    }

    public async Task StopAsync()
    {
        _listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();

        while (true)
        {
            try
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (await client.Client.ReceiveAsync(buff, SocketFlags.Peek) == 0)
                        break;
                }
                if (stream.DataAvailable)
                {
                    var record = await TLV.ReadRecordAsync(stream);

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

                    await TLV.WriteRecordAsync(stream, responseRecord);

                    Console.WriteLine($"Sent person: {response.FirstName} {response.LastName} ({response.Age})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                break;
            }
        }

        stream.Close();
        client.Close();
        Console.WriteLine("Client disconnected.");
    }
}