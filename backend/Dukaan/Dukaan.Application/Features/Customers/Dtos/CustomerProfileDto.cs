namespace Dukaan.Application.Features.Customers.Dtos;

public record CustomerProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Phone,
    string Email
);
