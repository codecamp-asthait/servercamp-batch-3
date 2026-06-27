using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery() : IQuery<ErrorOr<CustomerProfileDto>>;
