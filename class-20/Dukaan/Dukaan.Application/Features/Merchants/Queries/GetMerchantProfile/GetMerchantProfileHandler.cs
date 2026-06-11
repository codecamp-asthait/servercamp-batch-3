using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;

public class GetMerchantProfileHandler(IUserService userService, IRepository<Merchant> repository)
    : IQueryHandler<GetMerchantProfileQuery, ErrorOr<MerchantDto?>>
{
    public async Task<ErrorOr<MerchantDto?>> Handle(GetMerchantProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        
        if (userId is null)
            return MerchantErrors.NotFound;

        var merchant = await repository.FindAsync(m => m.ApplicationUserId == userId, trackChanges: false);
        var m = merchant.FirstOrDefault();
        
        if (m is null)
            return MerchantErrors.NotFound;

        return new MerchantDto(m.Id, m.StoreName, m.Slug, m.Description, m.LogoUrl);
    }
}
