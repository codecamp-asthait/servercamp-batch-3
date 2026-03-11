using System.Net;
using System.Text;
using System.Net.Sockets;

// A custom data structure to represent the HTTP request information,
// similar to .NET's HttpContext but much simpler.
class RequestContext
{
    public string Method { get; set; } = string.Empty; // GET, POST, etc.
    public string Path { get; set; } = string.Empty;   // The requested path, e.g., /hello
}

// TCP server class that listens for incoming TCP connections
class TcpServer
{
    private readonly int _port; // Port number where the server will listen

    // Constructor to initialize the server with a specific port
    public TcpServer(int port)
    {
        _port = port;
    }

    // Method to start the TCP server asynchronously
    public async Task StartAsync()
    {
        // Create a TCP listener bound to localhost (127.0.0.1) on the specified port
        var listener = new TcpListener(IPAddress.Loopback, _port);
        listener.Start(); // Start listening for incoming connections
        Console.WriteLine($"Server started on port {_port}");

        // Infinite loop to continuously accept new clients
        while (true)
        {
            // Accept a new client asynchronously
            var client = await listener.AcceptTcpClientAsync();

            // Handle the client connection in a background task so
            // the server can immediately accept other clients
            _ = Task.Run(() => HandleClient(client));
        }
    }

    // Method to handle a single client connection
    private async Task HandleClient(TcpClient client)
    {
        // Get the network stream to read/write data
        using var stream = client.GetStream();

        // Create a buffer to hold incoming bytes (max 1024 bytes at a time)
        var buffer = new byte[1024];

        // Read data from the client into the buffer asynchronously
        var byteCount = await stream.ReadAsync(buffer);

        // Convert the bytes to a string using UTF-8 encoding
        var requestText = Encoding.UTF8.GetString(buffer, 0, byteCount);

        // Naive parsing of HTTP request:
        // Split the request into lines by CRLF
        var lines = requestText.Split("\r\n");

        // The first line is usually like: "GET /path HTTP/1.1"
        var requestLine = lines[0].Split(' ');

        // Create a RequestContext object to store method and path
        var context = new RequestContext
        {
            Method = requestLine[0],
            Path = requestLine[1]
        };

        // Create a simple response that just echoes the requested path
        var responseText = $"You requested {context.Path}";

        // Convert the response to bytes
        // Include basic HTTP response headers
        var responseBytes = Encoding.UTF8.GetBytes(
            "HTTP/1.1 200 OK\r\n" +
            "Content-Length: " + responseText.Length + "\r\n\r\n" +
            responseText
        );

        // Send the response back to the client
        await stream.WriteAsync(responseBytes);

        // Close the connection after sending the response
        client.Close();
    }
}