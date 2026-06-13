using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Models;

namespace Dukaan.Application.Interfaces;

public interface IUserService
{
    Guid? GetCurrentUserId();
    Task<(Tenant tenant, Merchant Merchant, ApplicationUser User)?> GetMerchantByUserIdAsync(Guid userId);
    Task<AuthResponseDto?> LoginMerchantAsync(LoginRequestDto request);
    Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> CreateUserAsync(string email, string password, string role, Guid? tenantId = null);
}
