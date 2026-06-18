using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<ErrorOr<AuthDto>>;
