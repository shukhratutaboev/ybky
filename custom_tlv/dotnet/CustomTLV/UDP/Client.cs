using System.Net;
using System.Net.Sockets;
using CustomTLV.Encoders;

namespace CustomTLV.UDP;

public class Client
{
    private readonly IEncoder _encoder;
    private readonly UdpClient _client;

    public Client()
    {
        _encoder = new JsonEncoder();
        _client = new UdpClient();
    }

    public async Task SendAsync<T>(T data, string ip, int port)
    {
        var value = await _encoder.EncodeAsync(data);
        var record = new TLVRecord(1, (ushort)value.Length, value);

        var message = await TLV.WriteToByteAsync(record);

        _client.Send(message, message.Length, ip, port);
    }

    public async Task<T> ReceiveAsync<T>()
    {
        var server = new IPEndPoint(IPAddress.Any, 0);
        var record = await TLV.ReadFromByteAsync(_client.Receive(ref server));

        return await _encoder.DecodeAsync<T>(record.Value);
    }

    public async Task CloseAsync()
    {
        _client.Close();
    }
}