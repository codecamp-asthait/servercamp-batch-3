using Dukaan.Media.Application.Interfaces;
using ErrorOr;
using SkiaSharp;

namespace Dukaan.Media.Infrastructure.ImageProcessing;

public class SkiaSharpProcessor : IImageProcessor
{
    public async Task<ErrorOr<ProcessedImage>> ProcessAsync(Stream inputStream, string originalName)
    {
        try
        {
            using var inputData = SKData.Create(inputStream);
            using var bitmap = SKBitmap.Decode(inputData);
            if (bitmap is null)
                return Error.Failure("ImageProcessor.Failed", "Unable to decode image.");

            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            long originalFileSize = inputStream.Length;

            // Original variant (WebP quality 80)
            var originalStream = EncodeToWebP(bitmap, 80);

            // Display variant (max 800px)
            var displayStream = ResizeAndEncode(bitmap, 800, 800);

            // Thumbnail variant (max 200px)
            var thumbnailStream = ResizeAndEncode(bitmap, 200, 200);

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

    private static MemoryStream EncodeToWebP(SKBitmap bitmap, int quality)
    {
        var stream = new MemoryStream();
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Webp, quality);
        data.SaveTo(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream ResizeAndEncode(SKBitmap bitmap, int maxWidth, int maxHeight)
    {
        var (newWidth, newHeight) = CalculateSize(bitmap.Width, bitmap.Height, maxWidth, maxHeight);
        using var resized = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
        var stream = new MemoryStream();
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Webp, 80);
        data.SaveTo(stream);
        stream.Position = 0;
        return stream;
    }

    private static (int width, int height) CalculateSize(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        double ratio = Math.Min((double)maxWidth / originalWidth, (double)maxHeight / originalHeight);
        if (ratio >= 1)
            return (originalWidth, originalHeight);

        return ((int)(originalWidth * ratio), (int)(originalHeight * ratio));
    }
}
