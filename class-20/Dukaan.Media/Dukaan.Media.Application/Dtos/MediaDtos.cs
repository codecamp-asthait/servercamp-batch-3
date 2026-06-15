using Dukaan.Media.Domain.Enums;

namespace Dukaan.Media.Application.Dtos;

public record InitiateUploadRequest(string FileName, string ContentType, long TotalFileSize);

public record InitiateUploadResponse(Guid MediaId, int TotalChunks, long ChunkSize, string Message);

public record UploadChunkResponse(Guid MediaId, int ChunkIndex, int UploadedChunks, int TotalChunks, MediaStatus Status);

public record CompleteUploadResponse(Guid MediaId, MediaStatus Status, string Message = "Upload complete. Processing has started.");

public record MediaMetadataResponse(
    Guid Id,
    string OriginalFileName,
    MediaStatus Status,
    int UploadedChunks,
    int TotalChunks,
    DateTime CreatedAt,
    List<MediaVariantResponse>? Variants);

public record MediaVariantResponse(string VariantType, int Width, int Height, long FileSize);

public record MediaUrlResponse(string Url);
