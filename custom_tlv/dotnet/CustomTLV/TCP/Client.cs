using System.Net.Sockets;

namespace CustomTLV.TCP;

public class Client
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public Client(string ip, int port)
    {
        _client = new TcpClient();
        _client.Connect(ip, port);
        _stream = _client.GetStream();
    }

    public void Send<T>(T data)
    {
        var value = TLV.Encode(data);
        var record = new TLVRecord(1, (ushort)value.Length, value);

        TLV.WriteRecord(_stream, record);
    }

    public T Receive<T>()
    {
        var record = TLV.ReadRecord(_stream);

        return TLV.Decode<T>(record.Value);
    }

    public void Close()
    {
        _stream.Close();
        _client.Close();
    }
}