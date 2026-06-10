using dukaan.Application.DTOs;

namespace dukaan.Application.Services;

public interface ICustomerService
{
    Task<Guid> RegisterAsync(CustomerRegisterRequest request, Guid tenantId);
}
