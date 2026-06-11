namespace Dukaan.Application.Features.Customers.Dtos;

public record CustomerDto(
    Guid Id,
    Guid ApplicationUserId,
    Guid TenantId,
    string FirstName,
    string LastName,
    string Phone
);
