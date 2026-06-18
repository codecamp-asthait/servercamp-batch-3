using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

public class GetTenantIdFromSlugHandler(IRepository<Tenant> repository)
    : IQueryHandler<GetTenantIdFromSlugQuery, ErrorOr<Guid?>>
{
    public async Task<ErrorOr<Guid?>> Handle(GetTenantIdFromSlugQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.FindAsync(t => t.Slug == request.Slug, trackChanges: false, cancellationToken: cancellationToken);
        var tenant = result.FirstOrDefault();
        if (tenant is null) return TenantErrors.NotFound;
        return tenant.Id;
    }
}
