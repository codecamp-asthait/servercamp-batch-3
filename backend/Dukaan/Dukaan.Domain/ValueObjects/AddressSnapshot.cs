namespace Dukaan.Domain.ValueObjects;

public record AddressSnapshot(
    string Street,
    string City,
    string District,
    string PostalCode,
    string Phone
);
