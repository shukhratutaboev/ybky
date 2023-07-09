using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    public static void CreateServer(int port)
    {
        var serverIP = "127.0.0.8";

        TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), port);

        try
        {
            // Start listening for client connections
            listener.Start();

            Client.ConnectToServer(port);

            Console.WriteLine("Server started! Port: {0}", port);

            // Accept client connections
            TcpClient client = listener.AcceptTcpClient();

            Console.WriteLine("Client connected! Port: {0}", port);

            // Get the network stream for reading and writing
            NetworkStream stream = client.GetStream();

            // Receive data from the client
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            string message = Encoding.ASCII.GetString(data, 0, bytesRead);

            // Process the received data (you can add your custom logic here)
            string response = "Hello, client!";
            data = Encoding.ASCII.GetBytes(response);

            // Send the response back to the client
            stream.Write(data, 0, data.Length);

            Console.ReadKey();
            // Close the client socket and stream
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.Write("Error from server: " + e.Message);
            Console.WriteLine($" Port: {port}");
        }
        finally
        {
            // Stop listening and clean up the listener
            listener.Stop();
        }
    }
}