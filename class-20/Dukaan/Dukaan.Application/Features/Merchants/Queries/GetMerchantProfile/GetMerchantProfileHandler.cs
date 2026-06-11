using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Models;
using Dukaan.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;

public class GetMerchantProfileHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<Merchant> merchantRepository,
    IRepository<Tenant> tenantRepository)
    : IQueryHandler<GetMerchantProfileQuery, MerchantProfileResponseDto?>
{
    public async Task<MerchantProfileResponseDto?> Handle(GetMerchantProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return null;

        var merchants = await merchantRepository.FindAsync(m => m.ApplicationUserId == request.UserId);
        var merchant = merchants.FirstOrDefault();
        if (merchant == null) return null;

        var tenant = await tenantRepository.GetByIdAsync(merchant.TenantId);
        if (tenant == null) return null;

        return new MerchantProfileResponseDto(
            merchant.Id,
            merchant.TenantId,
            tenant.Slug,
            user.Email!,
            user.PhoneNumber!
        );
    }
}
