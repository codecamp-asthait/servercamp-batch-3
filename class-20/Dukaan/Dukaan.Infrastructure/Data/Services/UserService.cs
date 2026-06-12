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

    public async Task<(Merchant Merchant, ApplicationUser User)?> GetMerchantByEmailAsync(string email)
    {
        var result = await (
            from merchant in context.Merchants
            join user in context.Users on merchant.ApplicationUserId equals user.Id
            where user.Email == email && user.UserType == UserType.Merchant
            select new { merchant, user }
        ).FirstOrDefaultAsync();

        if (result is null) return null;
        return (result.merchant, result.user);
    }


    public async Task<(Customer Customer, ApplicationUser User)?> GetCustomerByEmailAsync(string email)
    {
        var result = await (
            from customer in context.Customers
            join user in context.Users on customer.ApplicationUserId equals user.Id
            where user.Email == email && user.UserType == UserType.Customer
            select new { customer, user }
        ).FirstOrDefaultAsync();

        if (result is null) return null;
        return (result.customer, result.user);
    }
    
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public async Task<AuthResponseDto?> LoginMerchantAsync(LoginRequestDto request)
    {
        var result = await GetMerchantByEmailAsync(request.Email);
        if (result is null) return null;

        var user = result.Value.User;
        var isValid = await applicationUserManager.CheckPasswordAsync(user, request.Password);
        if (!isValid) return null;

        var jwt = GenerateToken(user);
        var minutes = config["jwt:ExpireInMinutes"];
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(minutes!));
        return new AuthResponseDto(jwt, expiresAt);
    }

    public async Task<CustomerAuthResponseDto?> LoginCustomerAsync(CustomerLoginRequestDto request)
    {
        var result = await GetCustomerByEmailAsync(request.Email);
        if (result is null) return null;


        var user = result.Value.User;
        var isValid = await applicationUserManager.CheckPasswordAsync(user, request.Password);
        if (!isValid) return null;

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
