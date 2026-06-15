using Dukaan.Media.Application.Interfaces;

namespace Dukaan.Media.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;

    public Guid? GetTenantId() => _tenantId;

    public void SetTenantId(Guid tenantId) => _tenantId = tenantId;
}
