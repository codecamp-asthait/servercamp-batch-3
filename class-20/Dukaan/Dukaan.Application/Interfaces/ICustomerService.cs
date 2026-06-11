using Dukaan.Application.DTOs;

namespace Dukaan.Application.Services;

public interface ICustomerService
{
    Task<Guid> RegisterAsync(CustomerRegisterRequest request, Guid tenantId);
    Task<Guid?> GetCurrentCustomerIdAsync();
}
