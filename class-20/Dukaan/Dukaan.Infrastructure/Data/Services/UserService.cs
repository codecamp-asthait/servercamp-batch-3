using System.Text;
using System.Security.Claims;
using Dukaan.Application.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Models;
using Microsoft.Extensions.Configuration;

namespace Dukaan.Infrastructure.Data.Services;

public class UserService(
    IUserStore<ApplicationUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ApplicationUser> passwordHasher,
    IEnumerable<IUserValidator<ApplicationUser>> userValidators,
    IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<ApplicationUser>> logger,
    IConfiguration config) : UserManager<ApplicationUser>
(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services,
    logger), IUserService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await FindByEmailAsync(request.Email)
                   ?? throw new UnauthorizedAccessException("Invalid credentials");

        var isValid = await CheckPasswordAsync(user, request.Password);
        if (!isValid) throw new UnauthorizedAccessException("Invalid credentials");

        var jwt = GenerateToken(user);
        var minutes = config["jwt:ExpireInMinutes"];
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(minutes!));
        return new AuthResponseDto(jwt, expiresAt);
    }

    public async Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request, Guid tenantId)
    {
        var user = await FindByEmailAsync(request.Email);
        if (user == null || user.TenantId != tenantId || user.UserType != UserType.Customer) return null;
        if (!await CheckPasswordAsync(user, request.Password)) return null;

        return new CustomerAuthResponseDto(GenerateToken(user), user.Email!);
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <remarks>The generated token includes claims for the user's ID, email, and tenant ID. The token's
    /// expiration and signing credentials are determined by the current configuration settings. The caller is
    /// responsible for securely storing and transmitting the token.</remarks>
    /// <param name="user">The user for whom the JWT will be generated. Cannot be null.</param>
    /// <returns>A string containing the generated JWT for the specified user.</returns>
    private string GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new("tenant_id", user.TenantId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(config["jwt:ExpireInMinutes"]!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(securityToken);
    }
}
