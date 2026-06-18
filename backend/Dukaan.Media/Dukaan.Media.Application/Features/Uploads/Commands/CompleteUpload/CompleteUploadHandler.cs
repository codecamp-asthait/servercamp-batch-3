using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Media;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads.Commands.CompleteUpload;

public class CompleteUploadHandler(
    IRepository<MediaMetadata> mediaRepository,
    IJobDispatcher jobDispatcher)
    : ICommandHandler<CompleteUploadCommand, ErrorOr<CompleteUploadResponse>>
{
    public async Task<ErrorOr<CompleteUploadResponse>> Handle(
        CompleteUploadCommand command, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == command.MediaId, trackChanges: true, cancellationToken);

        if (media is null)
            return MediaErrors.NotFound;

        if (media.Status != MediaStatus.Uploading)
            return UploadErrors.NotInUploadingState;

        if (media.UploadedChunks != media.TotalChunks)
            return UploadErrors.IncompleteUpload;

        media.Status = MediaStatus.Pending;
        media.UpdatedAt = DateTime.UtcNow;
        await mediaRepository.SaveChangesAsync(cancellationToken);

        jobDispatcher.EnqueueProcessImage(command.MediaId, media.TenantId);

        return new CompleteUploadResponse(command.MediaId, media.Status);
    }
}
