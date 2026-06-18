using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads.Commands.InitiateUpload;

public class InitiateUploadHandler(IRepository<MediaMetadata> mediaRepository, ITenantProvider tenantProvider)
    : ICommandHandler<InitiateUploadCommand, ErrorOr<InitiateUploadResponse>>
{
    private const long ChunkSize = 5 * 1024 * 1024;

    public async Task<ErrorOr<InitiateUploadResponse>> Handle(
        InitiateUploadCommand command, CancellationToken cancellationToken)
    {
        var mediaId = Guid.NewGuid();
        var totalChunks = (int)Math.Ceiling((double)command.TotalFileSize / ChunkSize);

        var media = new MediaMetadata
        {
            Id = mediaId,
            TenantId = tenantProvider.GetTenantId() ?? Guid.Empty,
            OriginalFileName = command.FileName,
            ContentType = command.ContentType,
            TotalFileSize = command.TotalFileSize,
            ChunkSize = ChunkSize,
            TotalChunks = totalChunks,
            UploadedChunks = 0,
            Status = MediaStatus.Uploading
        };

        await mediaRepository.AddAsync(media, cancellationToken);
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return new InitiateUploadResponse(mediaId, totalChunks, ChunkSize, "Upload initiated.");
    }
}
