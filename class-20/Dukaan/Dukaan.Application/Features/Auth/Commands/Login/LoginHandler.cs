using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Auth;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public class LoginHandler(IUserService userService) : ICommandHandler<LoginCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await userService.LoginAsync(new LoginRequestDto(request.Email, request.Password));
            
            if (result is null)
                return AuthErrors.InvalidCredentials;
            
            return new AuthDto(result.Token, result.Expiration);
        }
        catch
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
