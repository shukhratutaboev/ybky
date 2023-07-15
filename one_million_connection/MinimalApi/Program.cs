using System.Net;
using System.Text;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();
app.MapGet("/", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
        {
            while (true)
            {
                await webSocket.SendAsync(Encoding.ASCII.GetBytes("1"), WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(1000);
            }
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

app.Run();