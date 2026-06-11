namespace Dukaan.Application.DTOs;

public record CustomerRegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone
);

public record CustomerLoginRequest(string Email, string Password);

public record CustomerAuthResponse(string Token, string Email);
