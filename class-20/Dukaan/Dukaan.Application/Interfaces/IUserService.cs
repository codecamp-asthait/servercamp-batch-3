using dukaan.Application.DTOs;
using Dukaan.Application.Dtos;
using Dukaan.Infrastructure.Data.Dtos;

namespace Dukaan.Application.Interfaces;

public interface IUserService
{
    public Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
    public Task<bool> CreateMerchantAsync(MerchantDto merchant, string password);
    public Task<CustomerAuthResponse?> LoginCustomerAsync(CustomerLoginRequest request, Guid tenantId);
}
