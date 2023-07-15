using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

public class WebSocketServer
{
    private IPAddress _ipAddress;
    private int _port;
    private TcpListener _tcpListener;

    public WebSocketServer(string ipAddress, int port)
    {
        _ipAddress = IPAddress.Parse(ipAddress);
        _port = port;
        _tcpListener = new TcpListener(_ipAddress, _port);
    }

    public void Start()
    {
        _tcpListener.Start();
        Console.WriteLine("Server started. Listening for incoming WebSocket connections...");

        var receiveThread = new Thread(new ThreadStart(ListenForClients));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private async void ListenForClients()
    {
        while (true)
        {
            Socket clientSocket = await _tcpListener.AcceptSocketAsync();
            Console.WriteLine("Client connected. Waiting for data...");

            ThreadPool.QueueUserWorkItem(c => HandleWebSocketCommunication(clientSocket));
        }
    }

    private void HandleWebSocketCommunication(Socket clientSocket)
    {
        try
        {
            // Perform WebSocket handshake
            string handshakeResponse = PerformWebSocketHandshake(clientSocket);

            // Send the WebSocket handshake response to the client
            SendResponse(clientSocket, handshakeResponse);

            // Start WebSocket communication
            byte[] buffer = new byte[1024];
            while (clientSocket.Connected)
            {
                int bytesRead = clientSocket.Receive(buffer);
                if (bytesRead > 0)
                {
                    // Process received data as WebSocket message
                    string message = DecodeWebSocketMessage(buffer, bytesRead);
                    Console.WriteLine("Received from client: " + message);

                    // Send a WebSocket message response
                    string response = "Hello client";
                    byte[] responseBytes = EncodeWebSocketMessage(response);
                    clientSocket.Send(responseBytes);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling WebSocket communication: " + ex.Message);
        }
        finally
        {
            clientSocket.Close();
        }
    }

    private string PerformWebSocketHandshake(Socket clientSocket)
    {
        // Read client's WebSocket handshake request
        byte[] requestBuffer = new byte[clientSocket.Available];
        clientSocket.Receive(requestBuffer);

        string request = Encoding.UTF8.GetString(requestBuffer);

        const string HandshakeGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        // Parse the client's handshake request
        string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
        string secWebSocketKey = string.Empty;

        // Find the value of the Sec-WebSocket-Key header
        foreach (string line in lines)
        {
            if (line.StartsWith("Sec-WebSocket-Key:"))
            {
                secWebSocketKey = line.Substring("Sec-WebSocket-Key:".Length).Trim();
                break;
            }
        }

        if (string.IsNullOrEmpty(secWebSocketKey))
        {
            throw new Exception("Invalid WebSocket handshake request");
        }

        // Generate the response key by appending the handshake GUID to the client's key and computing the SHA1 hash
        string combinedKey = secWebSocketKey + HandshakeGuid;
        byte[] combinedKeyBytes = Encoding.ASCII.GetBytes(combinedKey);
        byte[] responseKeyBytes = SHA1.Create().ComputeHash(combinedKeyBytes);
        string responseKey = Convert.ToBase64String(responseKeyBytes);

        // Construct the handshake response
        string response =
            "HTTP/1.1 101 Switching Protocols\r\n" +
            "Upgrade: websocket\r\n" +
            "Connection: Upgrade\r\n" +
            "Sec-WebSocket-Accept: " + responseKey + "\r\n" +
            "\r\n";

        return response;
    }

    private void SendResponse(Socket clientSocket, string response)
    {
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        clientSocket.Send(responseBytes);
    }

    private string DecodeWebSocketMessage(byte[] buffer, int length)
    {
        // Perform decoding of the received WebSocket message
        // The implementation depends on the WebSocket frame format and payload masking
        // Refer to the WebSocket protocol specifications for details

        // Example decoding logic
        byte[] payload = new byte[length - 6];
        Buffer.BlockCopy(buffer, 6, payload, 0, payload.Length);
        string message = Encoding.UTF8.GetString(payload);

        return message;
    }

    private byte[] EncodeWebSocketMessage(string message)
    {
        // Perform encoding of the WebSocket message to the frame format
        // The implementation depends on the WebSocket frame format and payload masking
        // Refer to the WebSocket protocol specifications for details

        // Example encoding logic
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        byte[] frame = new byte[messageBytes.Length + 2];
        frame[0] = 0x81;
        frame[1] = (byte)messageBytes.Length;
        Buffer.BlockCopy(messageBytes, 0, frame, 2, messageBytes.Length);

        return frame;
    }
}