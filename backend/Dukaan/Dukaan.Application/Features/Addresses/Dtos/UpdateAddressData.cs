namespace Dukaan.Application.Features.Addresses.Dtos;

public record UpdateAddressData(
    string Label,
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone
);
