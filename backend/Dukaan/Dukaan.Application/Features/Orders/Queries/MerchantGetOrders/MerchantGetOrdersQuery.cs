using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Orders.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Queries.MerchantGetOrders;

public sealed record MerchantGetOrdersQuery(int PageNumber, int PageSize)
    : IQuery<ErrorOr<PagedResponse<MerchantOrderSummaryDto>>>;
