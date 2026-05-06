namespace Dukaan.Application.Dtos;

/// <summary>
/// Represents the data required to request a user login, including the user's email address and password.
/// </summary>
/// <param name="Email">The email address associated with the user account. Cannot be null or empty.</param>
/// <param name="Password">The password for the user account. Cannot be null or empty.</param>
public record LoginRequestDTO(
    string Email,
    string Password
);