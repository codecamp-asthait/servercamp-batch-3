using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public class CheckSlugAvailabilityHandler(IRepository<Merchant> repository)
    : IQueryHandler<CheckSlugAvailabilityQuery, ErrorOr<bool>>
{
    public async Task<ErrorOr<bool>> Handle(CheckSlugAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var merchants = await repository.FindAsync(m => m.Slug == request.Slug, trackChanges: false);
        return !merchants.Any();
    }
}
