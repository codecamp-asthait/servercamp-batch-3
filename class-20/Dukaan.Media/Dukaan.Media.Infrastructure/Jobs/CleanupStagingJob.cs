using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using Dukaan.Media.Infrastructure.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Dukaan.Media.Infrastructure.Jobs;

public class CleanupStagingJob(
    MediaDbContext dbContext,
    IStorageProvider storageProvider)
{
    [AutomaticRetry(Attempts = 1)]
    [Queue("media")]
    public async Task ExecuteAsync()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var stale = await dbContext.MediaMetadata
            .IgnoreQueryFilters()
            .Where(m => m.Status == MediaStatus.Uploading && m.CreatedAt < cutoff)
            .ToListAsync();

        foreach (var media in stale)
        {
            var chunkKeys = await dbContext.MediaChunks
                .IgnoreQueryFilters()
                .Where(c => c.MediaId == media.Id)
                .Select(c => c.StorageKey)
                .ToListAsync();

            if (chunkKeys.Count > 0)
                await storageProvider.DeleteChunksAsync(chunkKeys);

            media.IsActive = false;
            media.UpdatedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync();
    }
}
