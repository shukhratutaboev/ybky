using System.Net.Sockets;
using CustomTLV.Encoders;

namespace CustomTLV.TCP;

public class Client
{
    private readonly IEncoder _encoder;
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public Client(string ip, int port)
    {
        _encoder = new ProtoBufEncoder();
        _client = new TcpClient();
        _client.Connect(ip, port);
        _stream = _client.GetStream();
    }

    public async Task SendAsync<T>(T data)
    {
        var value = await _encoder.EncodeAsync(data);
        var record = new TLVRecord(1, (ushort)value.Length, value);

        await TLV.WriteRecordAsync(_stream, record);
    }

    public async Task<T> ReceiveAsync<T>()
    {
        var record = await TLV.ReadRecordAsync(_stream);

        return await _encoder.DecodeAsync<T>(record.Value);
    }

    public async Task CloseAsync()
    {
        _stream.Close();
        _client.Close();
    }
}