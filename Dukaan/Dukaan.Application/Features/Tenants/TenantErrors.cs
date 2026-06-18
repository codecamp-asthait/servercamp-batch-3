using ErrorOr;

namespace Dukaan.Application.Features.Tenants;

public static class TenantErrors
{
    public static Error NotFound => Error.NotFound("Tenant.NotFound", "Tenant not found.");
    public static Error ContextMissing => Error.Unexpected("Tenant.ContextMissing", "Tenant context is missing.");
}
