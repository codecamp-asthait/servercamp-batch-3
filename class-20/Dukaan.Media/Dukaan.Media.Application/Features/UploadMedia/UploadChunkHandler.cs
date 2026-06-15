using Dukaan.Media.Application.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.UploadMedia;

public class UploadChunkHandler(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaChunk> chunkRepository,
    IStorageProvider storageProvider)
    : IRequestHandler<UploadChunkCommand, ErrorOr<UploadChunkResponse>>
{
    public async Task<ErrorOr<UploadChunkResponse>> Handle(
        UploadChunkCommand command, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == command.MediaId, trackChanges: true, cancellationToken);

        if (media is null)
            return Error.NotFound("Media.NotFound", "Media not found.");

        if (media.Status != MediaStatus.Uploading)
            return Error.Conflict("Media.InvalidStatus", "Media is not in uploading state.");

        if (command.ChunkIndex < 0 || command.ChunkIndex >= media.TotalChunks)
            return Error.Validation("Chunk.InvalidIndex", "Chunk index out of range.");

        var storageKey = $"chunk/{command.MediaId}/{command.ChunkIndex}";
        var uploadResult = await storageProvider.UploadChunkAsync(
            command.ChunkStream, storageKey, command.ContentType);

        if (uploadResult.IsError)
            return uploadResult.FirstError;

        await chunkRepository.AddAsync(new MediaChunk
        {
            Id = Guid.NewGuid(),
            MediaId = command.MediaId,
            ChunkIndex = command.ChunkIndex,
            StorageKey = storageKey,
            ChunkSize = command.ChunkLength
        }, cancellationToken);

        media.UploadedChunks++;
        media.UpdatedAt = DateTime.UtcNow;
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return new UploadChunkResponse(
            command.MediaId,
            command.ChunkIndex,
            media.UploadedChunks,
            media.TotalChunks,
            media.Status);
    }
}
