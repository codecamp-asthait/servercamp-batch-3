using Microsoft.AspNetCore.Http;
using Dukaan.Infrastructure.Services.Interfaces;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Implementation of ITenantProvider that retrieves the tenant ID from the authenticated user's claims or HTTP context items.
/// </summary>
/// <param name="httpContextAccessor">Accessor to the current HTTP context.</param>
public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    private const string ItemsKey = "TenantId";

    /// <summary>
    /// Retrieves the tenant ID from the current context.
    /// Checks the authenticated user's claims first, then falls back to HttpContext.Items.
    /// </summary>
    /// <returns>The parsed Guid if available; otherwise, null.</returns>
    public Guid? GetTenantId()
    {
        var tenantIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(tenantIdClaim, out var tenantId)) return tenantId;

        if (httpContextAccessor.HttpContext?.Items[ItemsKey] is Guid itemsTenantId)
            return itemsTenantId;

        return null;
    }

    /// <summary>
    /// Manually sets the tenant ID for the current request.
    /// </summary>
    /// <param name="tenantId">The tenant ID to set.</param>
    public void SetTenantId(Guid tenantId) =>
        httpContextAccessor.HttpContext!.Items[ItemsKey] = tenantId;
}
