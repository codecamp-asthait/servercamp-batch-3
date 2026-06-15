using Dukaan.Media.Application.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.UploadMedia;

public class CompleteUploadHandler(
    IRepository<MediaMetadata> mediaRepository,
    IJobDispatcher jobDispatcher)
    : IRequestHandler<CompleteUploadCommand, ErrorOr<CompleteUploadResponse>>
{
    public async Task<ErrorOr<CompleteUploadResponse>> Handle(
        CompleteUploadCommand command, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == command.MediaId, trackChanges: true, cancellationToken);

        if (media is null)
            return Error.NotFound("Media.NotFound", "Media not found.");

        if (media.Status != MediaStatus.Uploading)
            return Error.Conflict("Media.InvalidStatus", "Media is not in uploading state.");

        if (media.UploadedChunks != media.TotalChunks)
            return Error.Conflict("Media.IncompleteUpload",
                $"Expected {media.TotalChunks} chunks, got {media.UploadedChunks}.");

        media.Status = MediaStatus.Pending;
        media.UpdatedAt = DateTime.UtcNow;
        await mediaRepository.SaveChangesAsync(cancellationToken);

        jobDispatcher.EnqueueProcessImage(command.MediaId);

        return new CompleteUploadResponse(command.MediaId, media.Status);
    }
}
