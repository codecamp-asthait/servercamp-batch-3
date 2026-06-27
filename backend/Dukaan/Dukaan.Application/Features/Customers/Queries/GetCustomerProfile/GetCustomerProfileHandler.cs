using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Dtos;
using Dukaan.Application.Interfaces;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Queries.GetCustomerProfile;

public class GetCustomerProfileHandler(IUserService userService)
    : IQueryHandler<GetCustomerProfileQuery, ErrorOr<CustomerProfileDto>>
{
    public async Task<ErrorOr<CustomerProfileDto>> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        if (userId is null) return CustomerErrors.NotFound;

        var result = await userService.GetCustomerByUserIdAsync(userId.Value);
        if (result is null) return CustomerErrors.NotFound;

        var (customer, user) = result.Value;

        return new CustomerProfileDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Phone,
            user.Email ?? ""
        );
    }
}
