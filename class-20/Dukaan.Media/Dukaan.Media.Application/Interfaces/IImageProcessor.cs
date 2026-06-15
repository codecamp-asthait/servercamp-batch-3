using ErrorOr;

namespace Dukaan.Media.Application.Interfaces;

public interface IImageProcessor
{
    Task<ErrorOr<ProcessedImage>> ProcessAsync(Stream inputStream, string originalName);
}

public record ProcessedImage(
    Stream OriginalStream,
    Stream DisplayStream,
    Stream ThumbnailStream,
    int OriginalWidth,
    int OriginalHeight,
    long OriginalFileSize);
