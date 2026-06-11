using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public record CustomerLoginCommand(string Email, string Password, string TenantSlug) : ICommand<ErrorOr<CustomerAuthDto>>;
