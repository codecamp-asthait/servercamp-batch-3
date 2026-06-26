using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Orders.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Queries.GetOrder;

public sealed record GetOrderQuery(Guid OrderId)
    : IQuery<ErrorOr<OrderDto>>;
