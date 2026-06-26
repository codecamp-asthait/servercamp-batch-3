using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Orders.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(int PageNumber, int PageSize)
    : IQuery<ErrorOr<PagedResponse<OrderSummaryDto>>>;
