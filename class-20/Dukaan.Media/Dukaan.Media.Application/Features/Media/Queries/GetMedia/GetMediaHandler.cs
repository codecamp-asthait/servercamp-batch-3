using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Media.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Queries.GetMedia;

public class GetMediaHandler(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaVariant> variantRepository)
    : IQueryHandler<GetMediaQuery, ErrorOr<MediaMetadataResponse>>
{
    public async Task<ErrorOr<MediaMetadataResponse>> Handle(
        GetMediaQuery query, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == query.MediaId, cancellationToken: cancellationToken);

        if (media is null)
            return MediaErrors.NotFound;

        var variants = await variantRepository.FindAsync(
            v => v.MediaId == query.MediaId, cancellationToken: cancellationToken);

        return new MediaMetadataResponse(
            media.Id,
            media.OriginalFileName,
            media.Status,
            media.ImagePath,
            media.UploadedChunks,
            media.TotalChunks,
            media.CreatedAt,
            variants.Select(v => new MediaVariantResponse(v.VariantType, v.Width, v.Height, v.FileSize)).ToList());
    }
}
