using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public class CustomerLoginHandler(IUserService userService)
    : ICommandHandler<CustomerLoginCommand, CustomerAuthResponse?>
{
    public async Task<CustomerAuthResponse?> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.LoginCustomerAsync(
            new CustomerLoginRequestDto(request.Email, request.Password), request.TenantId);
        if (result == null) return null;
        return new CustomerAuthResponse(result.Token, result.Email);
    }
}
