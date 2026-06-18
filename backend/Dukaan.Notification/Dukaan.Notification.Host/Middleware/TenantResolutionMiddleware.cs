using Dukaan.Notification.Application.Interfaces;

namespace Dukaan.Notification.Host.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
                tenantProvider.SetTenantId(tenantId);
        }
        else if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenantId)
                 && Guid.TryParse(headerTenantId, out var tenantId))
        {
            tenantProvider.SetTenantId(tenantId);
        }

        await next(context);
    }
}
