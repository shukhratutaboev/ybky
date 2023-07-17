var udpServer = new CustomTLV.UDP.Server("127.0.0.2", 8080);
var tcpServer = new CustomTLV.TCP.Server("127.0.0.2", 8081);
var httpServer = new CustomTLV.HTTP.Server("127.0.0.2", 8082);

_ = Task.Run(udpServer.StartAsync);
_ = Task.Run(tcpServer.StartAsync);
_ = Task.Run(httpServer.StartAsync);

Console.ReadLine();

await udpServer.StopAsync();
await tcpServer.StopAsync();
await httpServer.StopAsync();