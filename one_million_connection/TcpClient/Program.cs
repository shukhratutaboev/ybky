// var i = 8080;
var tasks = new List<Thread>();
for (var i = 1000; i < 50000; i++)
{
    Console.WriteLine($"Starting server on port {i}...");
    var port = i;
    var task = new Thread(() => Server.CreateServer(port));
    tasks.Add(task);
}

// Parallel.ForEach(tasks, task => task.Start());
foreach (var task in tasks)
{
    task.Start();
}

Console.WriteLine("Servers started. Ok");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();