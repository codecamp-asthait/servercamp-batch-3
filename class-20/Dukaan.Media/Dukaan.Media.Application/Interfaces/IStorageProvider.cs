using ErrorOr;

namespace Dukaan.Media.Application.Interfaces;

public interface IStorageProvider
{
    Task<ErrorOr<string>> UploadAsync(Stream stream, string key, string contentType);
    Task<ErrorOr<string>> GetPresignedUrlAsync(string key, TimeSpan expiry);
    Task<ErrorOr<Deleted>> DeleteAsync(string key);
    Task<ErrorOr<Stream>> DownloadAsync(string key);
}
