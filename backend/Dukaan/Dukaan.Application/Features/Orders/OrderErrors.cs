using ErrorOr;

namespace Dukaan.Application.Features.Orders;

public static class OrderErrors
{
    public static Error CartEmpty => Error.Validation("Order.CartEmpty", "Cart is empty. Add items before placing an order.");
    public static Error AddressNotFound => Error.NotFound("Order.AddressNotFound", "One or both addresses were not found.");
    public static Error AddressNotOwned => Error.Forbidden("Order.AddressNotOwned", "One or both addresses do not belong to the customer.");
    public static Error ProductInactive => Error.Validation("Order.ProductInactive", "One or more products in the cart are no longer active.");
    public static Error InsufficientStock => Error.Validation("Order.InsufficientStock", "One or more products have insufficient stock.");
    public static Error CustomerNotFound => Error.Unauthorized("Order.CustomerNotFound", "Customer context not found.");
    public static Error NotFound => Error.NotFound("Order.NotFound", "Order not found.");
    public static Error AlreadyConfirmed => Error.Conflict("Order.AlreadyConfirmed", "Order is already confirmed.");
    public static Error AlreadyCancelled => Error.Conflict("Order.AlreadyCancelled", "Order is already cancelled.");
    public static Error InvalidStatusTransition => Error.Validation("Order.InvalidStatusTransition", "Invalid status transition.");
}
