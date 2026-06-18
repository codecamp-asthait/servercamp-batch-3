using Dukaan.Media.Domain.Enums;

namespace Dukaan.Media.Application.Features.Uploads.Dtos;

public record InitiateUploadResponse(Guid MediaId, int TotalChunks, long ChunkSize, string Message);

public record UploadChunkResponse(Guid MediaId, int ChunkIndex, int UploadedChunks, int TotalChunks, MediaStatus Status);

public record CompleteUploadResponse(Guid MediaId, MediaStatus Status, string Message = "Upload complete. Processing has started.");
