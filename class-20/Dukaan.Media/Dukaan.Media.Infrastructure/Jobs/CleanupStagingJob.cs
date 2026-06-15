using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using Hangfire;

namespace Dukaan.Media.Infrastructure.Jobs;

public class CleanupStagingJob(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaChunk> chunkRepository,
    IStorageProvider storageProvider)
{
    [AutomaticRetry(Attempts = 1)]
    [Queue("media")]
    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var stale = await mediaRepository.FindAsync(
            m => m.Status == MediaStatus.Uploading && m.CreatedAt < cutoff,
            trackChanges: true);

        foreach (var media in stale)
        {
            var chunks = await chunkRepository.FindAsync(c => c.MediaId == media.Id);
            var chunkKeys = chunks.Select(c => c.StorageKey).ToList();
            if (chunkKeys.Count > 0)
                await storageProvider.DeleteChunksAsync(chunkKeys);

            media.IsActive = false;
            media.UpdatedAt = DateTime.UtcNow;
        }

        await mediaRepository.SaveChangesAsync();
    }
}
