using learning_testing.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace learning_testing.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory that replaces the application's PostgreSQL
/// connection with a Testcontainers-managed PostgreSQL instance.
/// Implements IAsyncLifetime so the container starts before any test runs
/// and is disposed after all tests complete.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    /// <summary>PostgreSQL Testcontainer — runs a real Postgres in Docker for integration tests.</summary>
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    /// <summary>
    /// Overrides the default service registrations to swap the real DB connection
    /// string with the Testcontainer's ephemeral connection string.
    /// Also runs EF Core migrations to set up the schema.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the original DbContext registration (real DB connection string).
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register DbContext with the Testcontainer's connection string.
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_container.GetConnectionString());
            });

            // Run migrations so the test database has the correct schema.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        });
    }

    /// <summary>Starts the PostgreSQL container before tests run.</summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    /// <summary>Stops the container and disposes the factory after tests finish.</summary>
    public new async Task DisposeAsync()
    {
        await _container.StopAsync();
        await base.DisposeAsync();
    }
}
