using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Commands.UpdateAddress;

public record UpdateAddressCommand(
    string Label,
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone
) : ICommand<ErrorOr<AddressDto>>
{
    public Guid Id { get; init; }
}
