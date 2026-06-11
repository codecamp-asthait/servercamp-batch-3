using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;

public record GetCurrentCustomerIdQuery() : IQuery<Guid?>;
