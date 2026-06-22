using Dukaan.Domain.Enums;

namespace Dukaan.Application.Features.Addresses.Dtos;

public record AddressDto(
    Guid Id,
    string Label,
    AddressType Type,
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone,
    bool IsDefault
);
