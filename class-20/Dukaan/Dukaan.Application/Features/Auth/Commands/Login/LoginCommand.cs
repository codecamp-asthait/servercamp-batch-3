using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : ICommand<AuthResponse>;
