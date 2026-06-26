using Dukaan.Application.Interfaces;
using StackExchange.Redis;

namespace Dukaan.Infrastructure.Services;

public class EventBus(IConnectionMultiplexer redis) : IEventBus
{
    private const string StreamName = "order-events";

    public async Task PublishAsync(string eventName, IReadOnlyDictionary<string, string> data, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var entries = new List<NameValueEntry>(data.Count + 1) { new("event", eventName) };
        entries.AddRange(data.Select(kv => new NameValueEntry(kv.Key, kv.Value)));
        await db.StreamAddAsync(StreamName, entries.ToArray());
    }
}
