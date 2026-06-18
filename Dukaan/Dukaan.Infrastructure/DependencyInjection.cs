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
using Hangfire;
using Hangfire.PostgreSql;
using Dukaan.Infrastructure.Jobs;

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

        var mediaServiceBaseUrl = configuration["MediaService:BaseUrl"] ?? "http://dukaan-media:8080";
        services.AddHttpClient<IMediaService, MediaService>(client =>
        {
            client.BaseAddress = new Uri(mediaServiceBaseUrl);
        });

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(o =>
            {
                o.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            },
            new PostgreSqlStorageOptions { SchemaName = "hangfire" }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 2;
            options.Queues = ["media-resolution"];
        });

        services.AddHostedService<DatabaseMigrationHostedService>();
        services.AddHostedService<HangfireJobScheduler>();

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

public class HangfireJobScheduler : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<MediaResolutionJob>(
            "media-resolution",
            job => job.ExecuteAsync(),
            "*/30 * * * * *");

        Console.WriteLine("Hangfire recurring job 'media-resolution' scheduled every 30 seconds.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
