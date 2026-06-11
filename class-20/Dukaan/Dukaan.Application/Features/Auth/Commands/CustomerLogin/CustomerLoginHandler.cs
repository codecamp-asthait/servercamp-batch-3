using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Auth;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Features.Tenants;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public class CustomerLoginHandler(
    IUserService userService,
    IRepository<Tenant> tenantRepository,
    IRepository<Dukaan.Domain.Entities.Customer> customerRepository)
    : ICommandHandler<CustomerLoginCommand, ErrorOr<CustomerAuthDto>>
{
    public async Task<ErrorOr<CustomerAuthDto>> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tenants = await tenantRepository.FindAsync(t => t.Slug == request.TenantSlug, trackChanges: false);
            var tenant = tenants.FirstOrDefault();
            
            if (tenant is null)
                return TenantErrors.NotFound;

            var customers = await customerRepository.FindAsync(
                c => c.ApplicationUser!.Email == request.Email && c.TenantId == tenant.Id,
                trackChanges: false);
            var customer = customers.FirstOrDefault();
            
            if (customer is null)
                return AuthErrors.InvalidCredentials;

            var result = await userService.LoginAsync(new LoginRequestDto(request.Email, request.Password));
            
            if (result is null)
                return AuthErrors.InvalidCredentials;

            return new CustomerAuthDto(result.Token, result.Expiration, customer.Id);
        }
        catch
        {
            return AuthErrors.InvalidCredentials;
        }
    }
}
