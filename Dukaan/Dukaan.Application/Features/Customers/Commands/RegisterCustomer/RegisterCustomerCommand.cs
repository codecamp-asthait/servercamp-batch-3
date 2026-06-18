using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Commands.RegisterCustomer;

public record RegisterCustomerCommand(string Email, string Password, string FirstName, string LastName, string? Phone) : ICommand<ErrorOr<CustomerDto>>;
