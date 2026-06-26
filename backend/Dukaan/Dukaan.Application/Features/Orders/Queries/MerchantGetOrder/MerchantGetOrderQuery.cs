using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Orders.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Queries.MerchantGetOrder;

public sealed record MerchantGetOrderQuery(Guid OrderId) : IQuery<ErrorOr<MerchantOrderDto>>;
