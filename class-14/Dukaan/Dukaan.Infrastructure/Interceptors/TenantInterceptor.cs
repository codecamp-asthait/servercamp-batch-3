using Dukaan.Domain.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dukaan.Infrastructure.Interceptors;

public class TenantInterceptor(ITenantProvider tenantProvider) : SaveChangesInterceptor
{
    // intercept while saving
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }


    // intercept while saving
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var tenantId = tenantProvider.GetTenantId();

        // capture tenant base entities from change tracker
        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // Only set if not already set (allows manual override for registration/admin tasks)
                if (entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = tenantId
                            ?? throw new Exception("Tenant context missing while creating a tenant-scoped entity.");
                }
            }
        }
    }

}