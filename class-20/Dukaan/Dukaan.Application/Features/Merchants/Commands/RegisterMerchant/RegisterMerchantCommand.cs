using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;

public record RegisterMerchantCommand(
    string Email,
    string Password,
    string StoreName,
    string Slug,
    string? Description,
    string? LogoUrl) : ICommand<ErrorOr<MerchantDto>>;
