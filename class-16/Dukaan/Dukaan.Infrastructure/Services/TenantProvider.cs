using Microsoft.AspNetCore.Http;
using Dukaan.Infrastructure.Services.Interfaces;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Implementation of ITenantProvider that retrieves the tenant ID from the authenticated user's claims.
/// </summary>
/// <param name="httpContextAccessor">Accessor to the current HTTP context.</param>
public class TenantProvider(IHttpContextAccessor httpContextAccessor) : ITenantProvider
{
    /// <summary>
    /// Retrieves the tenant_id claim from the current user's principal.
    /// </summary>
    /// <returns>The parsed Guid if the claim exists and is valid; otherwise, null.</returns>
    public Guid? GetTenantId()
    {
        var tenantIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
    }
}