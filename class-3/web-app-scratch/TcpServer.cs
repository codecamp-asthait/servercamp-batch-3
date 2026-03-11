using System.Net;
using System.Text;
using System.Net.Sockets;

// A simple class that represents the HTTP request context.
// Similar to ASP.NET's HttpContext but extremely simplified.
// It only stores the HTTP method and the request path.
public class RequestContext
{
    // HTTP method (GET, POST, etc.)
    public string Method { get; set; } = string.Empty;

    // Requested URL path (e.g., /hello)
    public string Path { get; set; } = string.Empty;
}

// TCP server responsible for listening to incoming connections
// and forwarding requests to the Router.
public class TcpServer(int port, Router router)
{
    // Port where the server will listen
    private readonly int _port = port;

    // Router instance used to resolve incoming requests
    // to the correct handler.
    private readonly Router _router = router;

    // Starts the TCP server and begins listening for clients
    public async Task StartAsync()
    {
        // Create a TCP listener bound to localhost
        var listener = new TcpListener(IPAddress.Loopback, _port);

        // Start listening for incoming connections
        listener.Start();

        Console.WriteLine($"Server started on port {_port}");

        // Infinite loop to accept clients continuously
        while (true)
        {
            // Wait asynchronously for a new client connection
            var client = await listener.AcceptTcpClientAsync();

            // Handle the client in a background task so
            // the server can keep accepting new connections
            _ = Task.Run(() => HandleClient(client));
        }
    }

    // Handles a single client connection
    private async Task HandleClient(TcpClient client)
    {
        // Get the network stream to read/write data
        using var stream = client.GetStream();

        // Buffer to store incoming bytes
        var buffer = new byte[1024];

        // Read incoming request data
        var byteCount = await stream.ReadAsync(buffer);

        // Convert the received bytes to a string
        var requestText = Encoding.UTF8.GetString(buffer, 0, byteCount);

        // Very naive HTTP parsing
        // Example first line: "GET /path HTTP/1.1"
        var lines = requestText.Split("\r\n");

        // Extract request line parts
        var requestLine = lines[0].Split(' ');

        // Build the RequestContext object
        var context = new RequestContext
        {
            Method = requestLine[0], // GET, POST etc.
            Path = requestLine[1]    // Requested route
        };

        // Ask the router to resolve the request and return a response
        var responseText = _router.Resolve(context);

        // Construct a minimal HTTP response
        var responseBytes = Encoding.UTF8.GetBytes(
            "HTTP/1.1 200 OK\r\nContent-Length: " +
            responseText.Length +
            "\r\n\r\n" +
            responseText
        );

        // Send response back to the client
        await stream.WriteAsync(responseBytes);

        // Close the connection
        client.Close();
    }
}