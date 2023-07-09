var i = 8080;
// for (var i = 9000; i < 9002; i++)
// {
    Task.Run(() => 
    {
        Server.CreateServer(i);
        // Task.Delay(1000).Wait();
        // Client.ConnectToServer(i);
    });
// }

Console.WriteLine("Servers started.");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();