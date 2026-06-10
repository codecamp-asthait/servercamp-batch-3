using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;

namespace dukaan.Application.Services;

public class TenantService(IRepository<Tenant> repository) : ITenantService
{
    public async Task<Guid?> GetTenantIdFromSlug(string slug)
    {
        var result = await repository.FindAsync(t => t.Slug == slug);
        var tenant = result.FirstOrDefault();
        return tenant?.Id;
    }
}