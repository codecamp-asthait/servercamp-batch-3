using Dukaan.Domain.Entities;
using dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using dukaan.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.Model;
using Dukaan.Infrastructure.Data.DbContext;

namespace Dukaan.Infrastructure.Services;

public class MerchantService(
    IRepository<Tenant> tenantRepository,
    IRepository<Merchant> merchantRepository,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context) : IMerchantService
{
    public async Task<bool> IsSlugAvailable(string slug)
    {
        var existing = await tenantRepository.FindAsync(t => t.Slug == slug.ToLower());
        return !existing.Any();
    }

    public async Task<RegisterResponse> RegisterMerchant(MerchantRegisterRequest request)
    {
        await tenantRepository.BeginTransactionAsync();
        try
        {
            var tenant = new Tenant
            {
                StoreName = request.StoreName,
                Slug = request.Slug.ToLower(),
                Category = request.Category,
                Country = request.Country
            };

            await tenantRepository.AddAsync(tenant);
            await tenantRepository.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                TenantId = tenant.Id,
                UserType = UserType.Merchant
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            var merchant = new Merchant
            {
                ApplicationUserId = user.Id,
                TenantId = tenant.Id
            };
            await merchantRepository.AddAsync(merchant);

            await tenantRepository.SaveChangesAsync();
            await tenantRepository.CommitTransactionAsync();

            return new RegisterResponse(tenant.Id, tenant.StoreName);
        }
        catch
        {
            await tenantRepository.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<MerchantProfileDto?> GetMerchantProfile(Guid userId)
    {
        return await (
            from merchant in context.Merchants
            join user in context.Users on merchant.ApplicationUserId equals user.Id
            join tenant in context.Tenants on merchant.TenantId equals tenant.Id
            where user.Id == userId
            select new MerchantProfileDto(merchant.Id, merchant.TenantId, tenant.Slug, user.Email!, user.PhoneNumber!)
        ).FirstOrDefaultAsync();
    }
}
