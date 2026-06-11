using ErrorOr;

namespace Dukaan.Application.Features.Products;

public static class ProductErrors
{
    public static Error NotFound => Error.NotFound("Product.NotFound", "Product not found.");
    public static Error NotActive => Error.Validation("Product.NotActive", "Product is not active.");
    public static Error InsufficientStock => Error.Validation("Product.InsufficientStock", "Insufficient stock available.");
}
