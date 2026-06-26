using Dukaan.Application.Core.Abstractions;
using Dukaan.Domain.Enums;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : ICommand<ErrorOr<Success>>;
