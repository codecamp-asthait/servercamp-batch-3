using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;

namespace Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;

public record GetMerchantProfileQuery(Guid UserId) : IQuery<MerchantProfileResponseDto?>;
