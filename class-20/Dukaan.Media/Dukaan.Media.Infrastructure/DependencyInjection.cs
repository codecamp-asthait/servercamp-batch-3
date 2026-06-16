using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Infrastructure.Data;
using Dukaan.Media.Infrastructure.Data.Repositories;
using Dukaan.Media.Infrastructure.ImageProcessing;
using Dukaan.Media.Infrastructure.Jobs;
using Dukaan.Media.Infrastructure.Services;
using Dukaan.Media.Infrastructure.Storage;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;

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

        var minioEndpoint = configuration["MinIO:Endpoint"]!;
        var minioExternalEndpoint = configuration["MinIO:ExternalEndpoint"];
        var minioAccessKey = configuration["MinIO:AccessKey"]!;
        var minioSecretKey = configuration["MinIO:SecretKey"]!;
        var useSSL = bool.Parse(configuration["MinIO:UseSSL"] ?? "false");
        var bucketName = configuration["MinIO:BucketName"]!;

        services.AddSingleton(sp => new MinioClient()
            .WithEndpoint(minioEndpoint)
            .WithCredentials(minioAccessKey, minioSecretKey)
            .WithSSL(useSSL)
            .Build());

        services.AddScoped<IStorageProvider>(sp =>
            new MinioStorageProvider(
                sp.GetRequiredService<IMinioClient>(),
                bucketName,
                minioExternalEndpoint,
                useSSL));

        services.AddScoped<IImageProcessor, SkiaSharpProcessor>();
        services.AddScoped<IJobDispatcher, HangfireJobDispatcher>();

        services.AddHangfire(config => config
            .UsePostgreSqlStorage(o =>
            {
                o.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            },
            new PostgreSqlStorageOptions { SchemaName = "hangfire_media" }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 2;
            options.Queues = ["media"];
        });

        services.AddHostedService<HangfireJobScheduler>();
        services.AddHostedService<DatabaseMigrationHostedService>();

        return services;
    }
}

public class DatabaseMigrationHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
        var storageProvider = scope.ServiceProvider.GetRequiredService<IStorageProvider>();

        Console.WriteLine("Applying media database migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        Console.WriteLine("Media database migrations applied.");

        Console.WriteLine("Setting MinIO bucket to public read...");
        await storageProvider.SetBucketPublicReadAsync();
        Console.WriteLine("MinIO bucket configured.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
