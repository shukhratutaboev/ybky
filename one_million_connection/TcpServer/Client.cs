using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{
    public static void ConnectToServer(int port)
    {
        string serverIP = "127.0.0.3";
        string clientIp = "127.0.0.4";
        try
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp);

            // Bind the client to the local IP address and port
            clientSocket.Bind(new IPEndPoint(IPAddress.Parse(clientIp), port));

            // Connect to the server
            clientSocket.Connect(serverIP, port);

            // Send data to the server
            string message = "Hello, server!";
            byte[] data = Encoding.ASCII.GetBytes(message);
            clientSocket.Send(data);

            // Receive data from the server
            data = new byte[1024];
            int bytesRead = clientSocket.Receive(data);
            string response = Encoding.ASCII.GetString(data, 0, bytesRead);

            Console.ReadKey();
            // Close the client socket and stream
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}