using ErrorOr;
using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;

public class GetCurrentCustomerIdHandler(IUserService userService, IRepository<Customer> repository)
    : IQueryHandler<GetCurrentCustomerIdQuery, ErrorOr<Guid?>>
{
    public async Task<ErrorOr<Guid?>> Handle(GetCurrentCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId();
        if (userId is null) return CustomerErrors.NotFound;

        var customer = await repository.FindAsync(c => c.ApplicationUserId == userId, trackChanges: false, cancellationToken: cancellationToken);
        return customer.FirstOrDefault()?.Id;
    }
}
