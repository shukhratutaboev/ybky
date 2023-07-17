using System.Net;
using System.Net.Sockets;

namespace CustomTLV.TCP;

public class Server
{
    private readonly TcpListener _listener;

    public Server(string ip, int port)
    {
        _listener = new TcpListener(IPAddress.Parse(ip), port);
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine("Server started. Listening for incoming connections...");

        while (true)
        {
            var client = _listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            Task.Run(() => HandleClient(client));
        }
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();

        while (true)
        {
            try
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buff = new byte[1];
                    if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        break;
                }
                if (stream.DataAvailable)
                {
                    var record = TLV.ReadRecord(stream);

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

                    TLV.WriteRecord(stream, responseRecord);

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