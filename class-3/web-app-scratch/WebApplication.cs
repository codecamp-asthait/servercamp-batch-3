using System.Reflection;

// This is the main class of our mini web framework.
// It ties together the ServiceCollection (controllers) and Router (endpoints),
// and provides an easy API to register routes or controllers.
class MiniWebApplication(ServiceCollection services)
{
    // Store the service collection (contains discovered controllers)
    private readonly ServiceCollection _services = services;

    // Router handles mapping of incoming requests to endpoints
    private readonly Router _router = new();

    // Manually register a GET route
    public MiniWebApplication MapGet(string pattern, Func<RequestContext, string> handler)
    {
        _router.MapGet(pattern, handler);  // Add route to router
        return this;                       // Return "this" to allow chaining
    }

    // Manually register a POST route
    public MiniWebApplication MapPost(string pattern, Func<RequestContext, string> handler)
    {
        _router.MapPost(pattern, handler); // Add route to router
        return this;                       // Return "this" to allow chaining
    }

    // Automatically map controller methods as routes
    public MiniWebApplication MapControllers()
    {
        // Get all controller classes discovered by ServiceCollection
        var controllerTypes = _services.GetControllerTypes();

        // Loop through each controller class
        foreach (var controller in controllerTypes)
        {
            // Get all public instance methods in the controller
            var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                // Check if the method has an HTTP attribute (HttpGet or HttpPost)
                var attr = method.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();

                if (attr != null)
                {
                    // If it's a GET method, map it in the router
                    if (attr.Method == "GET")
                        _router.MapGet(attr.Path, ctx =>
                        {
                            // Create an instance of the controller
                            var instance = Activator.CreateInstance(controller);

                            // Invoke the method and get the result
                            var result = method.Invoke(instance, null);

                            // Return the result as a string
                            return result?.ToString() ?? "";
                        });

                    // If it's a POST method, map it in the router
                    else if (attr.Method == "POST")
                        _router.MapPost(attr.Path, ctx =>
                        {
                            var instance = Activator.CreateInstance(controller);
                            var result = method.Invoke(instance, null);
                            return result?.ToString() ?? "";
                        });
                }
            }
        }

        return this; // Allow method chaining
    }

    // Start the server and listen for incoming requests
    public async Task RunAsync(int port = 5000)
    {
        // Create a TCP server with the configured router
        var server = new TcpServer(port, _router);

        // Start listening asynchronously
        await server.StartAsync();
    }
}

// Builder for MiniWebApplication
class MiniWebApplicationBuilder
{
    // Create a ServiceCollection to hold controllers
    public ServiceCollection Services { get; } = new ServiceCollection(); 
    
    // Build a MiniWebApplication using the registered services
    public MiniWebApplication Build()
    {
        return new MiniWebApplication(Services);
    }
}

// Factory class to start building an app
static class WebApplicationFactory
{
    // Create a new builder
    public static MiniWebApplicationBuilder CreateBuilder()
    {
        return new MiniWebApplicationBuilder();
    }
}