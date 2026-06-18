namespace Dukaan.Application.Features.Merchants.Dtos;

public record MerchantDto(
    Guid Id,
    string StoreName,
    string Slug
);
