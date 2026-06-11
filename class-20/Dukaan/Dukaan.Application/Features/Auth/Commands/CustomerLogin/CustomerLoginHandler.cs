using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public class CustomerLoginHandler(
    UserManager<ApplicationUser> userManager,
    IConfiguration config)
    : ICommandHandler<CustomerLoginCommand, CustomerAuthResponse?>
{
    public async Task<CustomerAuthResponse?> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || user.TenantId != request.TenantId || user.UserType != UserType.Customer) return null;
        if (!await userManager.CheckPasswordAsync(user, request.Password)) return null;

        return new CustomerAuthResponse(GenerateToken(user), user.Email!);
    }

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
