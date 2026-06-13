namespace Dukaan.Application.Dtos;

public record AuthResponseDto(string Token, DateTime Expiration, Guid? TenantId = null);
