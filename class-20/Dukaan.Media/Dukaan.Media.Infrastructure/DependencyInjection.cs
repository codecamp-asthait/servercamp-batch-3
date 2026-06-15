using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Infrastructure.Data;
using Dukaan.Media.Infrastructure.Data.Repositories;
using Dukaan.Media.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dukaan.Media.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<MediaDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Dukaan.Media.Infrastructure"));
        });

        return services;
    }
}
