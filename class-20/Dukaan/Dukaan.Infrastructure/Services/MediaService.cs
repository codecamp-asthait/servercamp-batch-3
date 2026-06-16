using System.Net.Http.Json;
using Dukaan.Application.Interfaces;

namespace Dukaan.Infrastructure.Services;

public class MediaService(HttpClient httpClient) : IMediaService
{
    public async Task<MediaStatusResponse?> GetMediaStatusAsync(Guid mediaId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/media/{mediaId}", cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadFromJsonAsync<MediaApiResponse>(cancellationToken: cancellationToken);
        if (content is null) return null;

        return new MediaStatusResponse(content.Id, content.Status, content.ImagePath);
    }

    private record MediaApiResponse(Guid Id, string Status, string? ImagePath);
}
