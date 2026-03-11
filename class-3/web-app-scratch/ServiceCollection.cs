using System.Reflection;

// This class helps our mini web framework "find" all controllers automatically.
//
// In ASP.NET Core, this happens behind the scenes. Here, we do it manually
// so we can understand how controllers are discovered.
public class ServiceCollection
{
    // This list will store all the controller classes we find.
    private readonly List<Type> _controllerTypes = [];

    // Call this method to discover all controller classes in our project.
    // A "controller class" is any class whose name ends with "Controller"
    // (like UserController, ProductController, etc.)
    public void AddControllers()
    {
        // Get all the classes in the current project/assembly
        var controllers = Assembly.GetExecutingAssembly().GetTypes()
            // Only keep classes that end with "Controller"
            .Where(t => t.IsClass && t.Name.EndsWith("Controller"));

        // Add the found classes to our internal list
        _controllerTypes.AddRange(controllers);
    }

    // Returns the list of all controller classes we found.
    public List<Type> GetControllerTypes() => _controllerTypes;
}