namespace Dukaan.Application.Features.Customers.Dtos;

public record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone
);
