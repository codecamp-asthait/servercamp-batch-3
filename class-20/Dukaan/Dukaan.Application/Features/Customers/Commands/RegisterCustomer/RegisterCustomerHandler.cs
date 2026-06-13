using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Auth;
using Dukaan.Application.Features.Customers.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Observability;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Commands.RegisterCustomer;

public class RegisterCustomerHandler(
    IUserService userService,
    IRepository<Customer> repository)
    : ICommandHandler<RegisterCustomerCommand, ErrorOr<CustomerDto>>
{
    public async Task<ErrorOr<CustomerDto>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        await repository.BeginTransactionAsync();
        
        try
        {
            var existingUser = await userService.FindByEmailAsync(request.Email);
            if (existingUser is not null) return AuthErrors.EmailAlreadyRegistered;

            var user = await userService.CreateUserAsync(request.Email, request.Password, "Customer");
            if (user is null) return AuthErrors.IdentityCreationFailed;

            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone
            };

            await repository.AddAsync(customer);
            await repository.SaveChangesAsync();
            await repository.CommitTransactionAsync();

            DukaanMetrics.CustomerRegistrations.Add(1, DukaanMetrics.Tag("tenant_id", customer.TenantId));

            return new CustomerDto(customer.Id, customer.FirstName, customer.LastName, customer.Phone);
        }
        catch
        {
            await repository.RollbackTransactionAsync();
            throw;
        }
    }
}
