// Represents a single HTTP endpoint (route).
//
// An endpoint contains three main parts:
// 1. Path    → The route path (e.g., "/hello")
// 2. Method  → HTTP method (GET, POST, etc.)
// 3. Handler → Function that executes when the route matches
//
// This is conceptually similar to ASP.NET Core's internal
// Endpoint representation used by its routing system.
public class Endpoint(string path, string method, Func<RequestContext, string> handler)
{
    // The route path that this endpoint handles
    // Example: "/hello"
    public readonly string Path = path;

    // HTTP method for this endpoint
    // Example: GET, POST
    public readonly string Method = method;

    // Handler function that processes the request
    // It receives the RequestContext and returns a response string
    public readonly Func<RequestContext, string> Handler = handler;

    // Determines whether this endpoint matches the incoming request
    //
    // Matching rules:
    // 1. Request path must start with the endpoint path
    // 2. HTTP method must match (case-insensitive)
    //
    // Example:
    // Endpoint: GET /hello
    // Request:  GET /hello/world
    // Result:   true (because it starts with "/hello")
    public bool Matches(RequestContext ctx)
        => ctx.Path.StartsWith(Path) &&
           ctx.Method!.Equals(Method, StringComparison.OrdinalIgnoreCase);
}