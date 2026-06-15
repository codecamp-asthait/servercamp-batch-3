using Dukaan.Media.Infrastructure;

namespace Dukaan.Media.Host;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        return services;
    }
}
