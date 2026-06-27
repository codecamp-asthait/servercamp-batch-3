using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Infrastructure.Configuration;
using Dukaan.Notification.Infrastructure.Consumers;
using Dukaan.Notification.Infrastructure.Data;
using Dukaan.Notification.Infrastructure.Data.Repositories;
using Dukaan.Notification.Infrastructure.Dispatchers;
using Dukaan.Notification.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Dukaan.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ITenantProvider, TenantProvider>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<NotificationDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Dukaan.Notification.Infrastructure"));
        });

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));

        services.AddHostedService<OrderEventConsumer>();

        services.AddHostedService<DatabaseMigrationHostedService>();

        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));

        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<INotificationDispatchManager, NotificationDispatchManager>();
        services.AddScoped<INotificationDispatcher, InAppDispatcher>();
        services.AddScoped<INotificationDispatcher, SignalDispatcher>();
        services.AddScoped<INotificationDispatcher, EmailDispatcher>();

        return services;
    }
}

public class DatabaseMigrationHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

        Console.WriteLine("Applying notification database migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Notification database migrations applied.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
