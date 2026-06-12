using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public class CheckSlugAvailabilityHandler(IRepository<Tenant> repository)
    : IQueryHandler<CheckSlugAvailabilityQuery, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(
        CheckSlugAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var merchant = await repository.FindFirstAsync(m => m.Slug == request.Slug, trackChanges: false, cancellationToken: cancellationToken);
        return merchant is not null;
    }
}
