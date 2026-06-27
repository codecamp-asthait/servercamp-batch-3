using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Observability;
using Dukaan.Domain.Entities;
using Dukaan.Domain.ValueObjects;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using CartEntity = Dukaan.Domain.Entities.Cart;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderHandler(
    IRepository<OrderEntity> orderRepository,
    IRepository<CartEntity> cartRepository,
    IRepository<Address> addressRepository,
    IRepository<Product> productRepository,
    IOrderNumberService orderNumberService,
    IMediator mediator,
    IEventBus eventBus,
    ILogger<PlaceOrderHandler> logger,
    IUserService userService)
    : ICommandHandler<PlaceOrderCommand, ErrorOr<OrderDto>>
{
    public async Task<ErrorOr<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return OrderErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        if (customerId is null) return OrderErrors.CustomerNotFound;

        var cart = await cartRepository.FindFirstAsync(
            c => c.CustomerId == customerId,
            trackChanges: true,
            cancellationToken,
            c => c.Items!);

        if (cart is null || !cart.Items.Any()) return OrderErrors.CartEmpty;

        var billingAddress = await addressRepository.GetByIdAsync(request.BillingAddressId, cancellationToken: cancellationToken);
        var deliveryAddress = await addressRepository.GetByIdAsync(request.DeliveryAddressId, cancellationToken: cancellationToken);

        if (billingAddress is null || deliveryAddress is null) return OrderErrors.AddressNotFound;

        if (billingAddress.CustomerId != customerId || deliveryAddress.CustomerId != customerId)
            return OrderErrors.AddressNotOwned;

        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await productRepository.FindAsync(
            p => productIds.Contains(p.Id),
            trackChanges: true,
            cancellationToken);
        var productDict = products.ToDictionary(p => p.Id);

        var inactiveProducts = new List<Guid>();
        var insufficientStockItems = new List<(Guid ProductId, string Name, int Available, int Requested)>();

        foreach (var item in cart.Items)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
            {
                inactiveProducts.Add(item.ProductId);
                continue;
            }

            if (!product.IsActive)
            {
                inactiveProducts.Add(item.ProductId);
                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                insufficientStockItems.Add((product.Id, product.Name, product.StockQuantity, item.Quantity));
            }
        }

        if (inactiveProducts.Any()) return OrderErrors.ProductInactive;
        if (insufficientStockItems.Any()) return OrderErrors.InsufficientStock;

        try
        {
            await orderRepository.BeginTransactionAsync(cancellationToken);

            var (sequenceNumber, orderNumber) = await orderNumberService.GetNextOrderNumberAsync(cancellationToken);

            var billingSnapshot = new AddressSnapshot(
                billingAddress.Street,
                billingAddress.City,
                billingAddress.District,
                billingAddress.PostalCode,
                billingAddress.Phone);

            var deliverySnapshot = new AddressSnapshot(
                deliveryAddress.Street,
                deliveryAddress.City,
                deliveryAddress.District,
                deliveryAddress.PostalCode,
                deliveryAddress.Phone);

            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var cartItem in cart.Items)
            {
                var product = productDict[cartItem.ProductId];
                var itemSubtotal = cartItem.UnitPrice * cartItem.Quantity;
                subtotal += itemSubtotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = product.Name,
                    UnitPrice = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity,
                    Subtotal = itemSubtotal
                });

                product.StockQuantity -= cartItem.Quantity;
                productRepository.Update(product);
            }

            var order = new OrderEntity
            {
                CustomerId = customerId.Value,
                SequenceNumber = sequenceNumber,
                OrderNumber = orderNumber,
                Status = Domain.Enums.OrderStatus.Pending,
                BillingAddress = billingSnapshot,
                DeliveryAddress = deliverySnapshot,
                Subtotal = subtotal,
                Total = subtotal,
                Items = orderItems
            };

            await orderRepository.AddAsync(order, cancellationToken);

            cartRepository.Remove(cart);

            await orderRepository.SaveChangesAsync(cancellationToken);
            await orderRepository.CommitTransactionAsync(cancellationToken);

            DukaanMetrics.OrdersPlaced.Add(1, DukaanMetrics.Tag("tenant_id", order.TenantId));
            DukaanMetrics.OrderValue.Record((double)subtotal, DukaanMetrics.Tag("tenant_id", order.TenantId));

            try
            {
                var userId = userService.GetCurrentUserId();
                if (userId is not null)
                {
                    var customerResult = await userService.GetCustomerByUserIdAsync(userId.Value);
                    var customerEmail = customerResult?.User.Email ?? string.Empty;

                    await eventBus.PublishAsync("order-placed", new Dictionary<string, string>
                    {
                        ["tenant_id"] = order.TenantId.ToString(),
                        ["customer_id"] = userId.Value.ToString(),
                        ["customer_email"] = customerEmail,
                        ["notification_types"] = "in-app,email",
                        ["order_id"] = order.Id.ToString(),
                        ["order_display_id"] = order.OrderNumber
                    }, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish order-placed event for Order {OrderNumber}", order.OrderNumber);
            }

            return MapToDto(order);
        }
        catch
        {
            await orderRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }
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
