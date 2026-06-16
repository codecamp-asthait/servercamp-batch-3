using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Commands.DeleteMedia;

public class DeleteMediaHandler(
    IRepository<MediaMetadata> mediaRepository,
    IStorageProvider storageProvider)
    : ICommandHandler<DeleteMediaCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteMediaCommand command, CancellationToken cancellationToken)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == command.MediaId, cancellationToken: cancellationToken);

        if (media is null)
            return MediaErrors.NotFound;

        var deleteResult = await storageProvider.DeleteAsync($"media/{command.MediaId}");
        if (deleteResult.IsError)
            return deleteResult.FirstError;

        mediaRepository.Remove(media);
        await mediaRepository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
