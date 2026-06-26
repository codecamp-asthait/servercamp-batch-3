using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Queries.MerchantGetOrder;

public class MerchantGetOrderHandler(IRepository<OrderEntity> orderRepository)
    : IQueryHandler<MerchantGetOrderQuery, ErrorOr<MerchantOrderDto>>
{
    public async Task<ErrorOr<MerchantOrderDto>> Handle(
        MerchantGetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindFirstAsync(
            o => o.Id == request.OrderId,
            trackChanges: false,
            cancellationToken,
            o => o.Items,
            o => o.Customer);

        if (order is null)
            return OrderErrors.NotFound;

        return MapToDto(order);
    }

    private static MerchantOrderDto MapToDto(OrderEntity order)
    {
        return new MerchantOrderDto(
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
            $"{order.Customer.FirstName} {order.Customer.LastName}",
            order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.Subtotal)).ToList()
        );
    }
}
