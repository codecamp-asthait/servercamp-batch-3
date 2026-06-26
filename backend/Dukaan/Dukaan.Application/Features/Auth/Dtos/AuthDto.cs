namespace Dukaan.Application.Features.Auth.Dtos;

public record AuthDto(string Token, DateTime Expiration);

public record CustomerAuthDto(string Token, DateTime Expiration, Guid CustomerId);
