using StackExchange.Redis;

namespace Dukaan.Infrastructure.Services.Interfaces;

public interface IRedisService
{
    IDatabase Database { get; }
}
