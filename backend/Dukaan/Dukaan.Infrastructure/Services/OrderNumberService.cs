using StackExchange.Redis;
using Dukaan.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Services.Interfaces;

namespace Dukaan.Infrastructure.Services;

public class OrderNumberService(
    IRedisService redis,
    ITenantProvider tenantProvider,
    ApplicationDbContext dbContext) : IOrderNumberService
{
    private const string KeyPrefix = "order_seq:";

    public async Task<(long SequenceNumber, string OrderNumber)> GetNextOrderNumberAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantId = tenantProvider.GetTenantId();
        if (tenantId is null) throw new InvalidOperationException("Tenant ID is not available.");

        var db = redis.Database;
        var key = $"{KeyPrefix}{tenantId}";

        if (!await db.KeyExistsAsync(key))
        {
            await SeedFromDatabaseAsync(tenantId.Value, db, key, cancellationToken);
        }

        var sequence = await db.StringIncrementAsync(key);
        var orderNumber = $"ORD-{sequence:D6}";

        return (sequence, orderNumber);
    }

    private async Task SeedFromDatabaseAsync(
        Guid tenantId, IDatabase db, string key, CancellationToken cancellationToken)
    {
        var maxSequence = await dbContext.Orders
            .Where(o => o.TenantId == tenantId)
            .MaxAsync(o => (long?)o.SequenceNumber, cancellationToken) ?? 0;

        await db.StringSetAsync(key, maxSequence);
    }
}
