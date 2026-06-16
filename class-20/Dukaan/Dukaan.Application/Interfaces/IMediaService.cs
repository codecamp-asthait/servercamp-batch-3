namespace Dukaan.Application.Interfaces;

public interface IMediaService
{
    Task<MediaStatusResponse?> GetMediaStatusAsync(Guid mediaId, CancellationToken cancellationToken = default);
}

public record MediaStatusResponse(Guid Id, string Status, string? ImagePath);
