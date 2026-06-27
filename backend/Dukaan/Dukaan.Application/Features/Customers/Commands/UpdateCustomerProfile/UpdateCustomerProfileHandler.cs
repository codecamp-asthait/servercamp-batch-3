using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileHandler(
    IRepository<Customer> customerRepository,
    IUserService userService)
    : ICommandHandler<UpdateCustomerProfileCommand, ErrorOr<CustomerProfileDto>>
{
    public async Task<ErrorOr<CustomerProfileDto>> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        if (userId is null) return CustomerErrors.NotFound;

        var customer = await customerRepository.FindFirstAsync(
            c => c.ApplicationUserId == userId,
            trackChanges: true,
            cancellationToken);

        if (customer is null) return CustomerErrors.NotFound;

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Phone = request.Phone;

        await customerRepository.SaveChangesAsync(cancellationToken);

        var result = await userService.GetCustomerByUserIdAsync(userId.Value);
        var userEmail = result?.User.Email ?? "";

        return new CustomerProfileDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Phone,
            userEmail
        );
    }
}
