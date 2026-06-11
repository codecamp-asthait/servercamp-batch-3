using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;

public class GetCurrentCustomerIdHandler(IUserService userService, IRepository<Customer> repository)
    : IQueryHandler<GetCurrentCustomerIdQuery, ErrorOr<Guid?>>
{
    public async Task<ErrorOr<Guid?>> Handle(GetCurrentCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        
        if (userId is null)
            return CustomerErrors.NotFound;

        var customer = await repository.FindAsync(c => c.ApplicationUserId == userId, trackChanges: false);
        return customer.FirstOrDefault()?.Id;
    }
}
