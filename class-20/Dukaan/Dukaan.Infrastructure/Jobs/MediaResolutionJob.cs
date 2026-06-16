using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using Dukaan.Infrastructure.Data.DbContext;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dukaan.Infrastructure.Jobs;

public class MediaResolutionJob(
    ApplicationDbContext dbContext,
    IMediaService mediaService,
    ILogger<MediaResolutionJob> logger)
{
    [AutomaticRetry(Attempts = 3)]
    [Queue("media-resolution")]
    public async Task ExecuteAsync()
    {
        var products = await dbContext.Products
            .IgnoreQueryFilters()
            .Where(p => p.PendingMediaId != null)
            .ToListAsync();

        if (products.Count == 0)
            return;

        logger.LogInformation("Processing {Count} products with pending media", products.Count);

        foreach (var product in products)
        {
            try
            {
                var mediaStatus = await mediaService.GetMediaStatusAsync(product.PendingMediaId!.Value, product.TenantId);
                if (mediaStatus is null)
                {
                    logger.LogWarning("Could not fetch media status for product {ProductId}", product.Id);
                    continue;
                }

                if (mediaStatus.Status == 2 && !string.IsNullOrEmpty(mediaStatus.ImagePath))
                {
                    product.ImageUrl = mediaStatus.ImagePath;
                    product.PendingMediaId = null;
                    logger.LogInformation("Resolved media for product {ProductId}", product.Id);
                }
                else if (mediaStatus.Status == 3)
                {
                    product.PendingMediaId = null;
                    logger.LogWarning("Media processing failed for product {ProductId}", product.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error resolving media for product {ProductId}", product.Id);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
