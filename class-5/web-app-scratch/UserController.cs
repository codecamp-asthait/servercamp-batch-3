// A simple controller class that contains HTTP endpoint methods.
//
// In real ASP.NET Core, a controller groups related actions (methods)
// that handle HTTP requests, such as user management, products, etc.
//
// Here, we simulate this by using attributes to mark methods
// as HTTP GET or POST endpoints.
public class UserController
{
    // This method handles HTTP GET requests to the "/user" route.
    //
    // [HttpGet("/user")] is an attribute that marks this method as a GET endpoint
    // for the router. When the TCP server receives a GET request to "/user",
    // the router can detect this method via reflection and call it.
    [HttpGet("/user")]
    public string GetUser()
    {
        // This is the logic that runs when someone requests "/user"
        // In a real application, this might fetch data from a database.
        return "Fetching user from UserController";
    }
}