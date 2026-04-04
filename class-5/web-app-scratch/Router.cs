// The Router is responsible for mapping incoming HTTP requests
// to the correct endpoint handler.
//
// In ASP.NET Core this role is performed by the routing system
// (Endpoint Routing / Minimal API routing).
//
// Example idea:
// router.MapGet("/hello", handler)
// router.MapPost("/users", handler)
public class Router
{
    // Internal collection that stores all registered endpoints
    // Each endpoint represents a route + HTTP method + handler
    private readonly List<Endpoint> _endpoints = [];

    // Registers a GET endpoint
    //
    // Example usage:
    // router.MapGet("/hello", ctx => "Hello World");
    //
    // Parameters:
    // path    → URL route (e.g., /hello)
    // handler → Function that will execute when this route is matched
    public void MapGet(string path, Func<RequestContext, string> handler)
    {
        _endpoints.Add(new Endpoint(path, "GET", handler));
    }

    // Registers a POST endpoint
    //
    // Example usage:
    // router.MapPost("/users", ctx => "User created");
    //
    // Parameters:
    // path    → URL route
    // handler → Function that processes the request
    public void MapPost(string path, Func<RequestContext, string> handler)
    {
        _endpoints.Add(new Endpoint(path, "POST", handler));
    }

    // Resolves an incoming request by finding a matching endpoint
    //
    // Steps:
    // 1. Iterate through registered endpoints
    // 2. Find the first endpoint that matches the request
    // 3. Execute its handler
    // 4. Return the handler's response
    //
    // If no endpoint matches the request, return "404 Not Found"
    public string Resolve(RequestContext context)
    {
        // Try to find a matching endpoint
        var endpoint = _endpoints.FirstOrDefault(ep => ep.Matches(context));

        // If found, execute the handler
        // Otherwise return a 404 response
        return endpoint != null
            ? endpoint.Handler(context)
            : "404 Not Found";
    }
}