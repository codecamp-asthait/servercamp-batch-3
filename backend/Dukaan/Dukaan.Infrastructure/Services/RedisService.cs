using Dukaan.Infrastructure.Services.Interfaces;
using StackExchange.Redis;

namespace Dukaan.Infrastructure.Services;

public class RedisService(IConnectionMultiplexer connection) : IRedisService
{
    public IDatabase Database => connection.GetDatabase();
}
