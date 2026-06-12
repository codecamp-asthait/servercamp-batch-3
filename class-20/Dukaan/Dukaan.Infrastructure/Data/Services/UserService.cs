using System.Text;
using System.Security.Claims;
using Dukaan.Application.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Models;
using Dukaan.Domain.Entities;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Identity.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dukaan.Infrastructure.Data.Services;

public class UserService(
    IConfiguration config,
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IApplicationUserManagerAdapter applicationUserManager) : IUserService
{
    public Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return applicationUserManager.FindByEmailAsync(email);
    }

    public async Task<Customer?> GetCustomerByEmail(string email)
    {
        return await (
            from customer in context.Customers
            join user in context.Users on customer.ApplicationUserId equals user.Id
            where user.Email == email
            select customer
        ).FirstOrDefaultAsync();
    }
    
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await applicationUserManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        var isValid = await applicationUserManager.CheckPasswordAsync(user, request.Password);
        if (!isValid) throw new UnauthorizedAccessException("Invalid credentials");

        var jwt = GenerateToken(user);
        var minutes = config["jwt:ExpireInMinutes"];
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(minutes!));
        return new AuthResponseDto(jwt, expiresAt);
    }

    public async Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request)
    {
        var user = await applicationUserManager.FindByEmailAsync(request.Email);
        if (user == null || user.UserType != UserType.Customer) return null;
        if (!await applicationUserManager.CheckPasswordAsync(user, request.Password)) return null;

        var jwt = GenerateToken(user);
        var minutes = config["jwt:ExpireInMinutes"];
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(minutes!));

        return new CustomerAuthResponseDto(jwt, user.Id, expiresAt);
    }

    public async Task<ApplicationUser?> CreateUserAsync(string email, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            UserType = Enum.Parse<UserType>(role)
        };

        var result = await applicationUserManager.CreateAsync(user, password);
        return result.Succeeded ? user : null;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        return await applicationUserManager.GenerateEmailConfirmationTokenAsync(user);
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
