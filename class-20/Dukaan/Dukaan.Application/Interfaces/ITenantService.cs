namespace Dukaan.Application.Interfaces;

public interface ITenantService
{
    Task<Guid?> GetTenantIdFromSlug(string slug);
}