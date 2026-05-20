using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Dukaan.Application.Services;

/// <summary>
/// Provides authentication services for merchant users, including validating credentials and generating JSON Web Tokens
/// (JWT) for authenticated sessions.
/// </summary>
/// <remarks>This service is typically used to handle login operations and issue JWT tokens for authenticated
/// merchants. It relies on configuration settings for token generation and expiration. Thread safety depends on the
/// underlying dependencies.</remarks>
/// <param name="config">The application configuration instance used to retrieve authentication-related settings such as JWT keys and token
/// expiration.</param>
/// <param name="userService">The user manager responsible for accessing and validating merchant user accounts.</param>
public class AuthService(IConfiguration config, IUserService userService) : IAuthService
{
    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        return await userService.LoginAsync(request);
    }
}

// Infrastructure <- Application -> Domain
