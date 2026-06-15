using Dukaan.Media.Application.Interfaces;
using ErrorOr;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Dukaan.Media.Infrastructure.ImageProcessing;

public class ImageSharpProcessor : IImageProcessor
{
    public async Task<ErrorOr<ProcessedImage>> ProcessAsync(Stream inputStream, string originalName)
    {
        try
        {
            using var image = await Image.LoadAsync(inputStream).ConfigureAwait(false);
            
            // Strip metadata
            image.Metadata.ExifProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile = null;

            int originalWidth = image.Width;
            int originalHeight = image.Height;
            long originalFileSize = inputStream.Length;

            var originalStream = new MemoryStream();
            await image.SaveAsWebpAsync(originalStream, new WebpEncoder { Quality = 80 }).ConfigureAwait(false);
            originalStream.Position = 0;

            // Display Variant (max 800px)
            var displayStream = new MemoryStream();
            using (var displayImage = image.Clone(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(800, 800)
            })))
            {
                await displayImage.SaveAsWebpAsync(displayStream).ConfigureAwait(false);
            }
            displayStream.Position = 0;

            // Thumbnail Variant (max 200px)
            var thumbnailStream = new MemoryStream();
            using (var thumbnailImage = image.Clone(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(200, 200)
            })))
            {
                await thumbnailImage.SaveAsWebpAsync(thumbnailStream).ConfigureAwait(false);
            }
            thumbnailStream.Position = 0;

            return new ProcessedImage(
                originalStream,
                displayStream,
                thumbnailStream,
                originalWidth,
                originalHeight,
                originalFileSize);
        }
        catch (Exception ex)
        {
            return Error.Failure("ImageProcessor.Failed", ex.Message);
        }
    }
}
