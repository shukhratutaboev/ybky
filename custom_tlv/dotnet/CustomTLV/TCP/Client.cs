using System.Net.Sockets;

namespace CustomTLV.TCP;

public class Client
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public Client(string ip, int port)
    {
        _client = new TcpClient("127.0.0.2", 0);
        _client.Connect(ip, port);
        _stream = _client.GetStream();
    }

    public async Task SendAsync<T>(T data)
    {
        var value = await TLV.EncodeAsync(data);
        var record = new TLVRecord(1, (ushort)value.Length, value);

        await TLV.WriteRecordAsync(_stream, record);
    }

    public async Task<T> ReceiveAsync<T>()
    {
        var record = await TLV.ReadRecordAsync(_stream);

        return await TLV.DecodeAsync<T>(record.Value);
    }

    public async Task CloseAsync()
    {
        _stream.Close();
        _client.Close();
    }
}