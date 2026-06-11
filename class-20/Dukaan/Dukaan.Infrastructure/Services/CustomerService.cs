using Microsoft.AspNetCore.Identity;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.Model;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Dukaan.Domain.Entities;
using Dukaan.Application.DTOs;
using Dukaan.Application.Services;

namespace Dukaan.Infrastructure.Services;

public class CustomerService(
    UserManager<ApplicationUser> userManager,
    IRepository<Customer> customerRepository,
    IHttpContextAccessor httpContextAccessor) : ICustomerService
{
    public async Task<Guid?> GetCurrentCustomerIdAsync()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId)) return null;

        var results = await customerRepository.FindAsync(c => c.ApplicationUserId == userId);
        return results.FirstOrDefault()?.Id;
    }

    public async Task<Guid> RegisterAsync(CustomerRegisterRequest request, Guid tenantId)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null && existing.TenantId == tenantId)
            throw new InvalidOperationException("Email already registered in this store.");

        await customerRepository.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.Phone,
                TenantId = tenantId,
                UserType = UserType.Customer
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                TenantId = tenantId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone
            };

            await customerRepository.AddAsync(customer);
            await customerRepository.SaveChangesAsync();
            await customerRepository.CommitTransactionAsync();

            return customer.Id;
        }
        catch
        {
            await customerRepository.RollbackTransactionAsync();
            throw;
        }
    }
}
