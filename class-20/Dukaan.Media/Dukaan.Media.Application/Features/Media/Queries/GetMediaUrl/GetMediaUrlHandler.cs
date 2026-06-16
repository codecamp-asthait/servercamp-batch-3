using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Media.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Queries.GetMediaUrl;

public class GetMediaUrlHandler(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaVariant> variantRepository,
    IStorageProvider storageProvider)
    : IQueryHandler<GetMediaUrlQuery, ErrorOr<MediaUrlResponse>>
{
    public async Task<ErrorOr<MediaUrlResponse>> Handle(
        GetMediaUrlQuery query, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == query.MediaId, cancellationToken: cancellationToken);

        if (media is null)
            return MediaErrors.NotFound;

        var variant = await variantRepository.FindFirstAsync(
            v => v.MediaId == query.MediaId && v.VariantType == query.Variant,
            cancellationToken: cancellationToken);

        if (variant is null)
            return MediaErrors.VariantNotFound;

        var urlResult = await storageProvider.GetPresignedUrlAsync(
            variant.StorageKey, TimeSpan.FromHours(1));

        if (urlResult.IsError)
            return urlResult.FirstError;

        return new MediaUrlResponse(urlResult.Value);
    }
}
