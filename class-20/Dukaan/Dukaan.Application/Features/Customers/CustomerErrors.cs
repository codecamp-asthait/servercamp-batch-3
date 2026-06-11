using ErrorOr;

namespace Dukaan.Application.Features.Customers;

public static class CustomerErrors
{
    public static Error NotFound => Error.NotFound("Customer.NotFound", "Customer not found.");
    public static Error AlreadyRegistered => Error.Conflict("Customer.AlreadyRegistered", "Customer is already registered.");
}
