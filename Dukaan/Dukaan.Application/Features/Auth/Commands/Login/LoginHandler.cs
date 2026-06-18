using ErrorOr;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Observability;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public class LoginHandler(IUserService userService) : ICommandHandler<LoginCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var merchantLoginDto = new LoginRequestDto(request.Email, request.Password);
            var result = await userService.LoginMerchantAsync(merchantLoginDto);
            if (result is null)
            {
                DukaanMetrics.AuthFailures.Add(1);
                return AuthErrors.InvalidCredentials;
            }

            DukaanMetrics.AuthLogins.Add(1, DukaanMetrics.Tag("tenant_id", result.TenantId));
            return new AuthDto(result.Token, result.Expiration);
        }
        catch
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
