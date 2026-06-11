using Dukaan.Application.Dtos;

namespace Dukaan.Application.Services;

public interface IMerchantService
{
    Task<bool> IsSlugAvailable(string slug);
    Task<RegisterResponse> RegisterMerchant(MerchantRegisterRequest request);
    Task<MerchantProfileDto?> GetMerchantProfile(Guid userId);
}
