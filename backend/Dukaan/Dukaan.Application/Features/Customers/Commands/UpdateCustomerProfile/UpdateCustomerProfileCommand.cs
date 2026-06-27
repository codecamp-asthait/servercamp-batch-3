using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileCommand(
    string FirstName,
    string LastName,
    string? Phone
) : ICommand<ErrorOr<CustomerProfileDto>>;
