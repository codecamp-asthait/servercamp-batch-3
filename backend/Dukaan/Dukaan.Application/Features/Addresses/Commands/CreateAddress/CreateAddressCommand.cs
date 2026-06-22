using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Domain.Enums;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Commands.CreateAddress;

public record CreateAddressCommand(
    string Label,
    AddressType Type,
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone,
    bool IsDefault
) : ICommand<ErrorOr<AddressDto>>;
