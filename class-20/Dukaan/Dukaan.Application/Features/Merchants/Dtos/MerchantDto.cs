namespace Dukaan.Application.Features.Merchants.Dtos;

public record MerchantProfileResponseDto(
    Guid MerchantId,
    Guid TenantId,
    string Slug,
    string Email,
    string Phone
);

public record RegisterMerchantResponseDto(
    Guid TenantId,
    string StoreName
);
