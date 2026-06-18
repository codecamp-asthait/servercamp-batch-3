namespace Dukaan.Notification.Application.Interfaces;

public interface ITenantProvider
{
    Guid? GetTenantId();
    void SetTenantId(Guid tenantId);
}
