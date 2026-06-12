using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Models;

namespace Dukaan.Application.Interfaces;

public interface IUserService
{
    Guid? GetCurrentUserId();
    Task<Customer?> GetCustomerByEmail(string email);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> CreateUserAsync(string email, string password, string role);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
}
