using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public class CustomerLoginHandler(
    IUserService userService,
    IRepository<Customer> customerRepository)
    : ICommandHandler<CustomerLoginCommand, ErrorOr<CustomerAuthDto>>
{
    public async Task<ErrorOr<CustomerAuthDto>> Handle(CustomerLoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await userService.GetCustomerByEmail(request.Email);
            if (customer is null) return AuthErrors.InvalidCredentials;

            var result =
                await userService.LoginCustomerAsync(new CustomerLoginRequestDto(request.Email, request.Password));
            if (result is null) return AuthErrors.InvalidCredentials;

            return new CustomerAuthDto(result.Token, result.Expiration, customer.Id);
        }
        catch
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
