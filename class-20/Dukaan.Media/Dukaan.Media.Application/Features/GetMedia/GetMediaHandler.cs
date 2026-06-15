using Dukaan.Media.Application.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.GetMedia;

public class GetMediaHandler(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaVariant> variantRepository)
    : IRequestHandler<GetMediaQuery, ErrorOr<MediaMetadataResponse>>
{
    public async Task<ErrorOr<MediaMetadataResponse>> Handle(
        GetMediaQuery query, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == query.MediaId, cancellationToken: cancellationToken);

        if (media is null)
            return Error.NotFound("Media.NotFound", "Media not found.");

        var variants = await variantRepository.FindAsync(
            v => v.MediaId == query.MediaId, cancellationToken: cancellationToken);

        return new MediaMetadataResponse(
            media.Id,
            media.OriginalFileName,
            media.Status,
            media.UploadedChunks,
            media.TotalChunks,
            media.CreatedAt,
            variants.Select(v => new MediaVariantResponse(v.VariantType, v.Width, v.Height, v.FileSize)).ToList());
    }
}
