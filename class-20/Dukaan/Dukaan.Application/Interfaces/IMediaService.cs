namespace Dukaan.Application.Interfaces;

public interface IMediaService
{
    Task<MediaStatusResponse?> GetMediaStatusAsync(Guid mediaId, Guid tenantId, CancellationToken cancellationToken = default);
}

public record MediaStatusResponse(Guid Id, int Status, string? ImagePath);
