using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Enums;
using ErrorOr;
using OrderEntity = Dukaan.Domain.Entities.Order;

namespace Dukaan.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler(IRepository<OrderEntity> orderRepository)
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
