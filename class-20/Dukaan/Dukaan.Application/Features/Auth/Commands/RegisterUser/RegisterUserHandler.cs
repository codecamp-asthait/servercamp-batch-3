using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserHandler(IUserService userService)
    : ICommandHandler<RegisterUserCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userService.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return AuthErrors.EmailAlreadyRegistered;

        var user = await userService.CreateUserAsync(request.Email, request.Password, request.Role);
        if (user is null)
            return AuthErrors.IdentityCreationFailed;

        var token = await userService.GenerateEmailConfirmationTokenAsync(user);
        return new AuthDto(token, DateTime.UtcNow.AddDays(7));
    }
}
