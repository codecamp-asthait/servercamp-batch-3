using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Customers.Commands.RegisterCustomer;

public record RegisterCustomerCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    Guid TenantId
) : ICommand<Guid>;
