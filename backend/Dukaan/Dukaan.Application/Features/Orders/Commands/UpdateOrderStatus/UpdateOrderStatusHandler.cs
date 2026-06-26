using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Enums;
using ErrorOr;
using Microsoft.Extensions.Logging;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler(
    IRepository<OrderEntity> orderRepository,
    IEventBus eventBus,
    ILogger<UpdateOrderStatusHandler> logger)
    : ICommandHandler<UpdateOrderStatusCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, trackChanges: true, cancellationToken);

        if (order is null)
            return OrderErrors.NotFound;

        if (order.Status == request.NewStatus)
        {
            if (order.Status == OrderStatus.Confirmed)
                return OrderErrors.AlreadyConfirmed;
            if (order.Status == OrderStatus.Cancelled)
                return OrderErrors.AlreadyCancelled;
            return OrderErrors.InvalidStatusTransition;
        }

        if (!IsValidTransition(order.Status, request.NewStatus))
            return OrderErrors.InvalidStatusTransition;

        order.Status = request.NewStatus;
        await orderRepository.SaveChangesAsync(cancellationToken);

        var eventName = order.Status switch
        {
            OrderStatus.Confirmed => "order-confirmed",
            OrderStatus.Cancelled => "order-cancelled",
            _ => null
        };

        if (eventName is not null)
        {
            try
            {
                await eventBus.PublishAsync(eventName, new Dictionary<string, string>
                {
                    ["tenant_id"] = order.TenantId.ToString(),
                    ["customer_id"] = order.CustomerId.ToString(),
                    ["order_id"] = order.Id.ToString(),
                    ["order_display_id"] = order.OrderNumber
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish {EventName} event for Order {OrderNumber}", eventName, order.OrderNumber);
            }
        }

        return Result.Success;
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return current switch
        {
            OrderStatus.Pending => next is OrderStatus.Confirmed or OrderStatus.Cancelled,
            OrderStatus.Confirmed => next is OrderStatus.Cancelled,
            _ => false
        };
    }
}
