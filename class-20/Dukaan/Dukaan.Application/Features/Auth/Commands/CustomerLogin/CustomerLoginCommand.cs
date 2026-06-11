using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public record CustomerLoginCommand(
    string Email,
    string Password,
    Guid TenantId
) : ICommand<CustomerAuthResponse?>;
