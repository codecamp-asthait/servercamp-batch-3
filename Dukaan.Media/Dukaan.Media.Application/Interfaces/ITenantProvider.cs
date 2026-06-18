namespace Dukaan.Media.Application.Interfaces;

public interface ITenantProvider
{
    Guid? GetTenantId();
    void SetTenantId(Guid tenantId);
}
