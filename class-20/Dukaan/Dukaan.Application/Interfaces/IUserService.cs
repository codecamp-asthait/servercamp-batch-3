using Dukaan.Application.Dtos;
using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Interfaces;

public interface IUserService
{
    Guid? GetCurrentUserId();
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request, Guid tenantId);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> CreateUserAsync(string email, string password, string role);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
}
