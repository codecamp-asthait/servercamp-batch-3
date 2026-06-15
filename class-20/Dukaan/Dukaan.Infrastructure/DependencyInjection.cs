using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Dukaan.Infrastructure.Interceptors;
using Dukaan.Infrastructure.Data.Services;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Identity.Adapters;
using Dukaan.Infrastructure.Identity.Services;
using Dukaan.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Dukaan.Infrastructure.Identity.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;
using Dukaan.Infrastructure.Data;
using Microsoft.Extensions.Hosting;

namespace Dukaan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<TenantInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<TenantInterceptor>());
        });

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
        services.AddScoped<IApplicationUserManagerAdapter, ApplicationUserManagerAdapter>();

        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddHttpContextAccessor();

        services.AddHostedService<DatabaseMigrationHostedService>();

        return services;
    }

    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await DbSeeder.SeedAsync(context, userManager);
    }
}

public class DatabaseMigrationHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Applying database migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Database migrations applied.");

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await DbSeeder.SeedAsync(context, userManager);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
