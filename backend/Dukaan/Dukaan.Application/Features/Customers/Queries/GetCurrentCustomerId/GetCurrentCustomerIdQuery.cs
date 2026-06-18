using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;

public record GetCurrentCustomerIdQuery() : IQuery<ErrorOr<Guid?>>;
