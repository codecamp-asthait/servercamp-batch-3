using ErrorOr;

namespace Dukaan.Application.Features.Merchants;

public static class MerchantErrors
{
    public static Error NotFound => Error.NotFound("Merchant.NotFound", "Merchant not found.");
    public static Error SlugTaken => Error.Conflict("Merchant.SlugTaken", "Merchant slug is already taken.");
}
