using System.Net;
using System.Net.Sockets;

namespace CustomTLV.UDP;

public class Client
{
    private readonly UdpClient _client;

    public Client()
    {
        _client = new UdpClient();
    }

    public void Send<T>(T data, string ip, int port)
    {
        var value = TLV.Encode(data);
        var record = new TLVRecord(1, (ushort)value.Length, value);

        var message = TLV.WriteToByte(record);

        _client.Send(message, message.Length, ip, port);
    }

    public T Receive<T>()
    {
        var server = new IPEndPoint(IPAddress.Any, 0);
        var record = TLV.ReadFromByte(_client.Receive(ref server));

        return TLV.Decode<T>(record.Value);
    }

    public void Close()
    {
        _client.Close();
    }
}