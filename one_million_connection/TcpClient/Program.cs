var ipAddress = "0.0.0.0";
var port = 8000;

for (var i = 0; i < 10; i++)
{
    var server = new WebSocketServer(ipAddress, port + i);
    server.Start();
}

Console.ReadKey();