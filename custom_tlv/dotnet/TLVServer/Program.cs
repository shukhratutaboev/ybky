using CustomTLV.HTTP;

var server = new Server("127.0.0.2", 8080);

server.Start();

Console.ReadLine();

server.Stop();