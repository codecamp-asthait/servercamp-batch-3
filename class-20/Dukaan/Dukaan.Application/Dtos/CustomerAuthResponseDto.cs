namespace Dukaan.Application.Dtos;

public record CustomerAuthResponseDto(string Token, Guid UserId, DateTime Expiration);
