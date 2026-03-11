// Specifies that this attribute can only be used on methods.
// AllowMultiple = false means we cannot attach the same attribute
// multiple times to a single method.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]

// Base class for all HTTP method attributes (GET, POST, etc.)
//
// This class stores the common information required for routing:
// 1. HTTP Method (GET, POST)
// 2. Route Path (/users, /hello)
//
// ASP.NET Core has very similar attributes like:
// [HttpGet], [HttpPost], [HttpPut], etc.
public abstract class HttpMethodAttribute : Attribute
{
    // The route path that this endpoint should respond to
    // Example: "/hello"
    public string Path { get; }

    // HTTP method associated with this endpoint
    // Example: "GET", "POST"
    public string Method { get; }

    // Constructor used by derived attributes
    // Example:
    // HttpGetAttribute will pass ("GET", "/hello")
    protected HttpMethodAttribute(string method, string path)
    {
        Method = method;
        Path = path;
    }
}


// Attribute used to mark a method as handling HTTP GET requests
//
// Example usage:
//
// [HttpGet("/hello")]
// public string SayHello()
// {
//     return "Hello World";
// }
//
// When the router scans the controller methods using reflection,
// it can detect this attribute and automatically register the route.
public sealed class HttpGetAttribute : HttpMethodAttribute
{
    // Calls the base constructor with HTTP method = GET
    public HttpGetAttribute(string path) : base("GET", path) { }
}


// Attribute used to mark a method as handling HTTP POST requests
//
// Example usage:
//
// [HttpPost("/users")]
// public string CreateUser()
// {
//     return "User created";
// }
//
// During application startup, the router can inspect these
// attributes and automatically map routes.
public sealed class HttpPostAttribute : HttpMethodAttribute
{
    // Calls the base constructor with HTTP method = POST
    public HttpPostAttribute(string path) : base("POST", path) { }
}