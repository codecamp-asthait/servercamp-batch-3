using Dukaan.Media.Domain.Enums;

namespace Dukaan.Media.Application.Features.Media.Dtos;

public record MediaMetadataResponse(
    Guid Id,
    string OriginalFileName,
    MediaStatus Status,
    string? ImagePath,
    int UploadedChunks,
    int TotalChunks,
    DateTime CreatedAt,
    List<MediaVariantResponse>? Variants);

public record MediaVariantResponse(string VariantType, int Width, int Height, long FileSize);

public record MediaUrlResponse(string Url);
