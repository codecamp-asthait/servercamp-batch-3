using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Models;
using Dukaan.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;

public class RegisterMerchantHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<Tenant> tenantRepository,
    IRepository<Merchant> merchantRepository)
    : ICommandHandler<RegisterMerchantCommand, RegisterMerchantResponseDto>
{
    public async Task<RegisterMerchantResponseDto> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
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
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var merchant = new Merchant
            {
                ApplicationUserId = user.Id,
                TenantId = tenant.Id
            };
            await merchantRepository.AddAsync(merchant);

            await tenantRepository.SaveChangesAsync();
            await tenantRepository.CommitTransactionAsync();

            return new RegisterMerchantResponseDto(tenant.Id, tenant.StoreName);
        }
        catch
        {
            await tenantRepository.RollbackTransactionAsync();
            throw;
        }
    }
}
