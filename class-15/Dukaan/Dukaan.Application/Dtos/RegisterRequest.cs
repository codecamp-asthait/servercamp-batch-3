namespace Dukaan.Application.Dtos;

/// <summary>
/// Data Transfer Object (DTO) for registering a new merchant and their store.
/// </summary>
/// <remarks>
/// DTOs are used to define the contract between the API and its consumers.
/// They help in decoupling the external API structure from the internal Domain models.
/// </remarks>
public record MerchantRegisterRequest(
    string Email,
    string PhoneNumber,
    string Password,
    string StoreName,
    string Slug,
    string Category,
    string Country
);

/// <summary>
/// Response returned after a successful registration.
/// </summary>
public record RegisterResponse(
    Guid TenantId,
    string StoreName
);