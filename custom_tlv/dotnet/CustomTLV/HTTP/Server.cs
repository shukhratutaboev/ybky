using System.Net;
using System.Text;
using System.Text.Json;
using System;

namespace CustomTLV.HTTP;

public class Server
{
    private readonly HttpListener _listener;

    public Server(string ip, int port)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://{ip}:{port}/");
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("HTTP server started. Listening for incoming connections...");
        while (true)
        {
            var context = await _listener.GetContextAsync();

            Console.WriteLine("Client connected.");

            _ = Task.Run(async () => await HandleClientAsync(context));
        }
    }

    public async Task StopAsync()
    {
        _listener.Stop();
    }

    private static async Task HandleClientAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var requestBody = await ReadStreamAsync(request.InputStream);
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

        await response.OutputStream.WriteAsync(responseData);
        response.OutputStream.Close();
        response.Close();
    }

    private static async Task<byte[]> ReadStreamAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}