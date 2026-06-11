using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;

namespace Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;

public record RegisterMerchantCommand(
    string Email,
    string PhoneNumber,
    string Password,
    string StoreName,
    string Slug,
    string Category,
    string Country
) : ICommand<RegisterMerchantResponseDto>;
