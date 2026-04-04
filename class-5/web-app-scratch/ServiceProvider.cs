
/*
Custom ServiceProvider Documentation:
This class demonstrates a simplified **Dependency Injection (DI) container**.
It can register services and resolve them at runtime, including
their dependencies via constructor injection.

Key Features:
- Resolves registered services automatically.
- Supports **Transient** lifetime (new instance every time).
- Can find controller types (classes ending with 'Controller').
*/

public class ServiceProvider
{
    // List of all registered services
    private readonly List<ServiceDescriptor> _services;

    /// <summary>
    /// Constructor initializes the ServiceProvider with registered services.
    /// </summary>
    /// <param name="services">List of service descriptors</param>
    public ServiceProvider(List<ServiceDescriptor> services)
    {
        _services = services;
    }

    /// <summary>
    /// Generic method to resolve a service by type.
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Resolved service instance</returns>
    public T GetService<T>() => (T)GetService(typeof(T));

    /// <summary>
    /// Resolves a service given its type.
    /// </summary>
    /// <param name="serviceType">Type of the service to resolve</param>
    /// <returns>Resolved service instance</returns>
    public object GetService(Type serviceType)
    {
        // Find the registered service descriptor
        var descriptor = _services.FirstOrDefault(x => x.ServiceType == serviceType)
            ?? throw new Exception($"Service {serviceType.Name} isn't registered");

        // Resolve service based on its lifetime
        return descriptor.LifeTime switch
        {
            ServiceLifetime.Transient => CreateInstance(descriptor.ImplementationType),
            _ => throw new Exception("Unknown lifetime")
        };
    }

    /// <summary>
    /// Creates an instance of a type using constructor injection.
    /// </summary>
    /// <param name="implType">Implementation type to instantiate</param>
    /// <returns>New instance with dependencies injected</returns>
    private object CreateInstance(Type implType)
    {
        // Get the first constructor of the type
        var ctor = implType.GetConstructors();
        var firstConstructor = ctor.First();

        // Resolve all constructor parameters (dependencies)
        var deps = firstConstructor.GetParameters()
            .Select(p => GetService(p.ParameterType))
            .ToArray();

        // Create instance using resolved dependencies
        return Activator.CreateInstance(implType, deps)!;
    }

    /// <summary>
    /// Returns a list of registered controller types.
    /// Controllers are identified by class names ending with 'Controller'.
    /// </summary>
    /// <returns>List of controller types</returns>
    public List<Type> GetControllerTypes()
    {
        return _services
            .Where(d => d.ServiceType.Name.EndsWith("Controller"))
            .Select(d => d.ServiceType)
            .ToList();
    }
}