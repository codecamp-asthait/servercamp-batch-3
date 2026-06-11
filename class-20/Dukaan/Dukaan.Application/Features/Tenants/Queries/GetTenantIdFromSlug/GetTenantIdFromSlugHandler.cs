using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

public class GetTenantIdFromSlugHandler(IRepository<Tenant> repository)
    : IQueryHandler<GetTenantIdFromSlugQuery, Guid?>
{
    public async Task<Guid?> Handle(GetTenantIdFromSlugQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.FindAsync(t => t.Slug == request.Slug);
        return result.FirstOrDefault()?.Id;
    }
}
