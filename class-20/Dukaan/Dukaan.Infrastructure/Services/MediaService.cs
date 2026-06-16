using System.Net.Http.Json;
using Dukaan.Application.Interfaces;

namespace Dukaan.Infrastructure.Services;

public class MediaService(HttpClient httpClient) : IMediaService
{
    public async Task<MediaStatusResponse?> GetMediaStatusAsync(Guid mediaId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/media/{mediaId}");
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
        
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadFromJsonAsync<MediaApiResponse>(cancellationToken: cancellationToken);
        if (content is null) return null;

        return new MediaStatusResponse(content.Id, content.Status, content.ImagePath);
    }

    private record MediaApiResponse(Guid Id, int Status, string? ImagePath);
}
