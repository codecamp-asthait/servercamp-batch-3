using ErrorOr;

namespace Dukaan.Application.Features.Addresses;

public static class AddressErrors
{
    public static Error NotFound => Error.NotFound("Address.NotFound", "Address not found.");
    public static Error NotOwned => Error.Forbidden("Address.NotOwned", "Address does not belong to customer.");
    public static Error CannotDeleteDefault => Error.Validation("Address.CannotDeleteDefault", "Cannot delete default address. Set another address as default first.");
    public static Error CustomerNotFound => Error.Unauthorized("Address.CustomerNotFound", "Customer context not found.");
}
