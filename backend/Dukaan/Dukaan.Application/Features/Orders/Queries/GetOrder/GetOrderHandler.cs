using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;
using MediatR;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Queries.GetOrder;

public class GetOrderHandler(
    IRepository<OrderEntity> orderRepository,
    IMediator mediator)
    : IQueryHandler<GetOrderQuery, ErrorOr<OrderDto>>
{
    public async Task<ErrorOr<OrderDto>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError)
            return OrderErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;

        var order = await orderRepository.FindFirstAsync(
            o => o.Id == request.OrderId && o.CustomerId == customerId,
            trackChanges: false,
            cancellationToken,
            o => o.Items);

        if (order is null)
            return OrderErrors.NotFound;

        return MapToDto(order);
    }

    private static OrderDto MapToDto(OrderEntity order)
    {
        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.Status,
            new AddressSnapshotDto(
                order.BillingAddress.Street,
                order.BillingAddress.City,
                order.BillingAddress.District,
                order.BillingAddress.PostalCode,
                order.BillingAddress.Phone),
            new AddressSnapshotDto(
                order.DeliveryAddress.Street,
                order.DeliveryAddress.City,
                order.DeliveryAddress.District,
                order.DeliveryAddress.PostalCode,
                order.DeliveryAddress.Phone),
            order.Subtotal,
            order.Total,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Subtotal)).ToList()
        );
    }
}
