using ErrorOr;

namespace Dukaan.Application.Features.Cart;

public static class CartErrors
{
    public static Error NotFound => Error.NotFound("Cart.NotFound", "Cart not found.");
    public static Error ItemNotFound => Error.NotFound("Cart.ItemNotFound", "Cart item not found.");
    public static Error CustomerNotFound => Error.Unauthorized("Cart.CustomerNotFound", "Customer context not found.");
}
