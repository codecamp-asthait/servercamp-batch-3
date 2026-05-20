namespace Dukaan.Application.Dtos;

/// <summary>
/// Represents the result of a successful authentication operation, including the issued token and its expiration time.
/// </summary>
/// <param name="Token">The authentication token issued to the client. This token is typically used to authorize subsequent requests.</param>
/// <param name="Expiration">The date and time when the authentication token expires. After this time, the token is no longer valid.</param>
public record AuthResponseDTO(
    string Token,
    DateTime Expiration
);
