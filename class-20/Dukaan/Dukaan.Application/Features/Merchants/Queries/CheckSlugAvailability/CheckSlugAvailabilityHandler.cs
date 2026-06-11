using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public class CheckSlugAvailabilityHandler(
    IRepository<Tenant> tenantRepository)
    : IQueryHandler<CheckSlugAvailabilityQuery, bool>
{
    public async Task<bool> Handle(CheckSlugAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var existing = await tenantRepository.FindAsync(t => t.Slug == request.Slug.ToLower());
        return !existing.Any();
    }
}
