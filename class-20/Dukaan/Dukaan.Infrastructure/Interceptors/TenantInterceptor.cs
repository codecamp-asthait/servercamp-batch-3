using Dukaan.Domain.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dukaan.Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor that automatically sets the TenantId on entities implementing ITenantEntity.
/// </summary>
/// <param name="tenantProvider">The service used to retrieve the current tenant context.</param>
public class TenantInterceptor(ITenantProvider tenantProvider) : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the SaveChanges operation to update tenant-scoped entities.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts the SaveChangesAsync operation to update tenant-scoped entities.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Automatically populates the TenantId for newly added entities that implement ITenantEntity.
    /// </summary>
    /// <param name="context">The current database context.</param>
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