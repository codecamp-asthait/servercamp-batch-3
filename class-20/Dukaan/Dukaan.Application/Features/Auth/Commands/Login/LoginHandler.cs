using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public class LoginHandler(IUserService userService) : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(new LoginRequestDto(request.Email, request.Password));
        return new AuthResponse(result.Token, result.Expiration);
    }
}
