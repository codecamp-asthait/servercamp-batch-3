namespace Dukaan.Application.Features.Auth.Dtos;

public record AuthResponse(
    string Token,
    DateTime Expiration
);

public record CustomerAuthResponse(
    string Token,
    string Email
);

public record RegisterUserResponse(
    Guid UserId,
    string Email
);
