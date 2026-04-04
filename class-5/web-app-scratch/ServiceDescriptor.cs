public class ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
{
    public Type ServiceType { get; } = serviceType;
    public Type ImplementationType { get; } = implementationType;
    public ServiceLifetime LifeTime { get; set; } = lifetime;
}

public enum ServiceLifetime
{
    Singleton,
    Scoped,
    Transient
}