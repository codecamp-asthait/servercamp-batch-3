using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Orders.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(
    Guid BillingAddressId,
    Guid DeliveryAddressId
) : ICommand<ErrorOr<OrderDto>>;
