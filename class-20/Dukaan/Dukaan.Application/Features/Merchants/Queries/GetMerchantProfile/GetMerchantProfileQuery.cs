using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Merchants.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;

public record GetMerchantProfileQuery() : IQuery<ErrorOr<MerchantDto?>>;
