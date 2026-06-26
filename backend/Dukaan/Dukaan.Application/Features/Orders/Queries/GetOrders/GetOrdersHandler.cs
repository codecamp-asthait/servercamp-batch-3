using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersHandler(
    IRepository<Order> orderRepository,
    IMediator mediator)
    : IQueryHandler<GetOrdersQuery, ErrorOr<PagedResponse<OrderSummaryDto>>>
{
    public async Task<ErrorOr<PagedResponse<OrderSummaryDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError)
            return OrderErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;

        var (orders, totalCount) = await orderRepository.GetPagedAsync(
            o => o.CustomerId == customerId,
            request.PageNumber,
            request.PageSize,
            trackChanges: false,
            cancellationToken,
            q => q.OrderByDescending(o => o.CreatedAt),
            o => o.Items);

        var items = orders.Select(o => new OrderSummaryDto(
            o.Id,
            o.OrderNumber,
            o.Status,
            o.Total,
            o.Items.Count,
            o.CreatedAt));

        return new PagedResponse<OrderSummaryDto>(
            items, totalCount, request.PageNumber, request.PageSize);
    }
}
