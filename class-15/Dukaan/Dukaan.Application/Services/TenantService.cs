using Dukaan.Application.Dtos;
using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.Dtos;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Service responsible for handling tenant-related business logic.
/// </summary>
/// <remarks>
/// This service orchestrates the creation of a new Store (Tenant) and the primary user (Merchant).
/// </remarks>
public class TenantService(
    IRepository<Tenant> tenantRepository,
    IUserService userService)
{
    /// <summary>
    /// Registers a new merchant along with their store (tenant).
    /// </summary>
    /// <param name="request">The registration details.</param>
    /// <returns>A response containing the new Tenant ID and Store Name.</returns>
    /// <exception cref="Exception">Thrown when merchant creation fails.</exception>
    /// <remarks>
    /// This method demonstrates a composite operation: 
    /// 1. Create a Tenant.
    /// 2. Create a Merchant linked to that Tenant.
    /// </remarks>
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

            var merchantDto = new MerchantDto
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                TenantId = tenant.Id
            };

            var isMerchantCreated = await userService.CreateMerchantAsync(merchantDto, request.Password);

            if (!isMerchantCreated)
            {
                await tenantRepository.RollbackTransactionAsync();
                throw new Exception("Merchant creation failed");
            }

            await tenantRepository.CommitTransactionAsync();
            return new RegisterResponse(tenant.Id, tenant.StoreName);
        }
        catch (Exception)
        {
            await tenantRepository.RollbackTransactionAsync();
            throw;
        }
    }
}