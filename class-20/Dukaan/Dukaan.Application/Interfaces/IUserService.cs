using Dukaan.Application.Dtos;
using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request, Guid tenantId);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
}
