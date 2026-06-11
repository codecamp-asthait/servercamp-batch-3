using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Models;
using Dukaan.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Features.Customers.Commands.RegisterCustomer;

public class RegisterCustomerHandler(
    UserManager<ApplicationUser> userManager,
    IRepository<Customer> customerRepository)
    : ICommandHandler<RegisterCustomerCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null && existing.TenantId == request.TenantId)
            throw new InvalidOperationException("Email already registered in this store.");

        await customerRepository.BeginTransactionAsync();
        try
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.Phone,
                TenantId = request.TenantId,
                UserType = UserType.Customer
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                TenantId = request.TenantId,
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
