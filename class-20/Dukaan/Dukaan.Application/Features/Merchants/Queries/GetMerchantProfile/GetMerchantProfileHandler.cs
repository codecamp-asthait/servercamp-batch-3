using ErrorOr;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;

namespace Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;

public class GetMerchantProfileHandler(IUserService userService)
    : IQueryHandler<GetMerchantProfileQuery, ErrorOr<MerchantDto?>>
{
    public async Task<ErrorOr<MerchantDto?>> Handle(GetMerchantProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        if (userId is null) return MerchantErrors.NotFound;

        var result = await userService.GetMerchantByUserIdAsync(userId.Value);
        var tenant = result?.tenant;
        var merchant = result?.Merchant;
        if (tenant is null || merchant is null) return MerchantErrors.NotFound;

        return new MerchantDto(merchant.Id, tenant.StoreName, tenant.Slug);
    }
}
