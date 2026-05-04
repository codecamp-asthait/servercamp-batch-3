using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Dukaan.Infrastructure.Data.Model;
using Dukaan.Infrastructure.Data.Repositories;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Service responsible for handling tenant-related business logic.
/// </summary>
/// <remarks>
/// This service orchestrates the creation of a new Store (Tenant) and the primary user (Merchant).
/// </remarks>
public class TenantService(
    Repository<Tenant> tenantRepository,
    UserManager<Merchant> userManager)
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
        var tenant = new Tenant
        {
            StoreName = request.StoreName,
            Slug = request.Slug.ToLower(),
            Category = request.Category,
            Country = request.Country
        };

        await tenantRepository.AddAsync(tenant);
        await tenantRepository.SaveChangesAsync();

        var merchant = new Merchant
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            TenantId = tenant.Id
        };

        var result = await userManager.CreateAsync(merchant, request.Password);

        return !result.Succeeded
            ? throw new Exception("Merchant creation failed")
            : new RegisterResponse(tenant.Id, tenant.StoreName);
    }
}