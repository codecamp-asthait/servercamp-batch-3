using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Queries.GetAddressById;

public record GetAddressByIdQuery(Guid Id) : IQuery<ErrorOr<AddressDto>>;
