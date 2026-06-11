using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserHandler(
    UserManager<ApplicationUser> userManager)
    : ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            UserType = UserType.Merchant
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return new RegisterUserResponse(user.Id, user.Email!);
    }
}
