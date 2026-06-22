using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Queries.GetCustomerAddresses;

public record GetCustomerAddressesQuery() : IQuery<ErrorOr<List<AddressDto>>>;
