using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Dukaan.Media.Domain.Enums;
using Hangfire;

namespace Dukaan.Media.Infrastructure.Jobs;

public class ProcessImageJob(
    IRepository<MediaMetadata> mediaRepository,
    IRepository<MediaChunk> chunkRepository,
    IRepository<MediaVariant> variantRepository,
    IStorageProvider storageProvider,
    IImageProcessor imageProcessor)
{
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [10, 30, 60])]
    [Queue("media")]
    public async Task ExecuteAsync(Guid mediaId)
    {
        var media = await mediaRepository.FindFirstAsync(
            m => m.Id == mediaId,
            trackChanges: true);

        if (media is null || media.Status != MediaStatus.Pending)
            return;

        if (media.UploadedChunks != media.TotalChunks)
            return;

        try
        {
            var chunks = await chunkRepository.FindAsync(
                c => c.MediaId == mediaId);

            var chunkKeys = chunks
                .OrderBy(c => c.ChunkIndex)
                .Select(c => c.StorageKey)
                .ToList();

            var combineResult = await storageProvider.CombineChunksAsync(chunkKeys);
            if (combineResult.IsError)
                throw new InvalidOperationException(combineResult.FirstError.Description);

            var processResult = await imageProcessor.ProcessAsync(combineResult.Value, media.OriginalFileName);
            if (processResult.IsError)
                throw new InvalidOperationException(processResult.FirstError.Description);

            var processed = processResult.Value;
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var basePath = $"media/{media.TenantId}/{year}/{month}/{media.Id}";

            media.ImagePath = basePath;

            var variants = new[]
            {
                ("original", processed.OriginalStream, processed.OriginalWidth, processed.OriginalHeight, processed.OriginalFileSize),
                ("display",  processed.DisplayStream,  0, 0, 0L),
                ("thumbnail",processed.ThumbnailStream, 0, 0, 0L),
            };

            foreach (var (variantType, stream, width, height, fileSize) in variants)
            {
                var key = $"{basePath}/{variantType}.webp";
                var uploadResult = await storageProvider.UploadAsync(stream, key, "image/webp");
                if (uploadResult.IsError)
                    throw new InvalidOperationException(uploadResult.FirstError.Description);

                await variantRepository.AddAsync(new MediaVariant
                {
                    Id = Guid.NewGuid(),
                    MediaId = mediaId,
                    VariantType = variantType,
                    StorageKey = key,
                    Width = width,
                    Height = height,
                    FileSize = fileSize,
                });

                await stream.DisposeAsync();
            }

            await storageProvider.DeleteChunksAsync(chunkKeys);

            media.Status = MediaStatus.Completed;
            media.UpdatedAt = DateTime.UtcNow;
            await mediaRepository.SaveChangesAsync();
        }
        catch
        {
            media.Status = MediaStatus.Failed;
            media.UpdatedAt = DateTime.UtcNow;
            await mediaRepository.SaveChangesAsync();
            throw;
        }
    }
}
