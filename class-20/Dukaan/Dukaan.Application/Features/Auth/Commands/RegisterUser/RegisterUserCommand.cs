using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string Role) : ICommand<ErrorOr<AuthDto>>;
