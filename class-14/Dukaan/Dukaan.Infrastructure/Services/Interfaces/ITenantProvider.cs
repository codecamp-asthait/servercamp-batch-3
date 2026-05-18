namespace Dukaan.Infrastructure.Services.Interfaces;

public interface ITenantProvider
{
    Guid? GetTenantId();
}