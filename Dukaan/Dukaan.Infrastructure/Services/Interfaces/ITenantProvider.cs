namespace Dukaan.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface for services that provide the current tenant's context.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the current tenant's unique identifier.
    /// </summary>
    /// <returns>The tenant ID if available; otherwise, null.</returns>
    Guid? GetTenantId();

    void SetTenantId(Guid tenantId);
}