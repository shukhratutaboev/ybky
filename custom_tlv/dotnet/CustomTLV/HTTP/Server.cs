using System.Net;
using System.Text;
using System.Text.Json;

namespace CustomTLV.HTTP;

public class Server
{
    private readonly HttpListener _listener;

    public Server(string ip, int port)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://{ip}:{port}/");
    }

    public void Start()
    {
        _listener.Start();
        while (true)
        {
            var context = _listener.GetContext();

            Console.WriteLine("Client connected.");

            Task.Run(() => HandleClient(context));
        }
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void HandleClient(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var requestBody = ReadStream(request.InputStream);
        var requestData = Encoding.UTF8.GetString(requestBody);
        var recievedObject = JsonSerializer.Deserialize<Person>(requestData);

        Console.WriteLine($"Received person: {recievedObject.FirstName} {recievedObject.LastName} ({recievedObject.Age})");

        var data = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = recievedObject.Age + 1
        };

        var responseBody = JsonSerializer.Serialize(data);
        var responseData = Encoding.UTF8.GetBytes(responseBody);

        response.ContentType = "application/json";
        response.ContentLength64 = responseData.Length;
        response.StatusCode = 200;

        response.OutputStream.Write(responseData, 0, responseData.Length);
        response.OutputStream.Close();
        response.Close();
    }

    private byte[] ReadStream(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}