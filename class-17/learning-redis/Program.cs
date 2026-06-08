// Entry point for the learning-redis ASP.NET Core application.
// Demonstrates core Redis data structures via REST endpoints:
//   - Strings, Lists, Sets, Sorted Sets, Hashes, Pub/Sub, Distributed Locks
using learning_redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")
                           ?? throw new InvalidOperationException("Redis connection string is missing.");
    var lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
    return lazy.Value;
});

builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>("Redis");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapHealthChecks("/health");
app.MapControllers();

// Subscribe to the "system-notifications" channel so every running instance
// logs messages published via POST /pubsub-example/notify.
var subscriber = app.Services
    .GetRequiredService<IConnectionMultiplexer>()
    .GetSubscriber();

await subscriber.SubscribeAsync(RedisChannel.Literal("system-notifications"), (channel, message) =>
{
    // This callback fires on every published message — in every running instance.
    app.Logger.LogInformation("[PubSub] {Channel}: {Message}", channel, message);
});

app.Run();