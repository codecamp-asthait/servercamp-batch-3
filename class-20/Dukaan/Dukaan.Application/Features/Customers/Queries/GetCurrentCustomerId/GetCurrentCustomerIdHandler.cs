using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;

public class GetCurrentCustomerIdHandler(
    IHttpContextAccessor httpContextAccessor,
    IRepository<Customer> customerRepository)
    : IQueryHandler<GetCurrentCustomerIdQuery, Guid?>
{
    public async Task<Guid?> Handle(GetCurrentCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId)) return null;

        var results = await customerRepository.FindAsync(c => c.ApplicationUserId == userId);
        return results.FirstOrDefault()?.Id;
    }
}
