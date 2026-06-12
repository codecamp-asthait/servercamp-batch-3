using ErrorOr;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public class LoginHandler(IUserService userService) : ICommandHandler<LoginCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var merchantLoginDto = new LoginRequestDto(request.Email, request.Password);
            var result = await userService.LoginMerchantAsync(merchantLoginDto);
            if (result is null) return AuthErrors.InvalidCredentials;
            
            return new AuthDto(result.Token, result.Expiration);
        }
        catch
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
