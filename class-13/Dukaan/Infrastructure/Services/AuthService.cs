using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Provides authentication services for merchant users, including validating credentials and generating JSON Web Tokens
/// (JWT) for authenticated sessions.
/// </summary>
/// <remarks>This service is typically used to handle login operations and issue JWT tokens for authenticated
/// merchants. It relies on configuration settings for token generation and expiration. Thread safety depends on the
/// underlying dependencies.</remarks>
/// <param name="config">The application configuration instance used to retrieve authentication-related settings such as JWT keys and token
/// expiration.</param>
/// <param name="userManager">The user manager responsible for accessing and validating merchant user accounts.</param>
public class AuthService(IConfiguration config, UserManager<Merchant> userManager) : IAuthService
{
    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var isValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isValid)
            throw new UnauthorizedAccessException("Invalid credentials");

        var jwt = GenerateToken(user);

        var minutes = config["jwt:ExpireInMinutes"];
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(minutes!));

        return new AuthResponseDTO(jwt,expiresAt);
    }


    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified merchant user.
    /// </summary>
    /// <remarks>The generated token includes claims for the user's ID, email, and tenant ID. The token's
    /// expiration and signing credentials are determined by the current configuration settings. The caller is
    /// responsible for securely storing and transmitting the token.</remarks>
    /// <param name="user">The merchant user for whom the JWT will be generated. Cannot be null.</param>
    /// <returns>A string containing the generated JWT for the specified user.</returns>
    private string GenerateToken(Merchant user)
    {
        var key = config["Jwt:Key"];

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("email", user.Email!),
            new Claim("tenant_id", user.TenantId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(config["jwt:ExpireInMinutes"]!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();

        var securityToken =  handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(securityToken);
    }
}
