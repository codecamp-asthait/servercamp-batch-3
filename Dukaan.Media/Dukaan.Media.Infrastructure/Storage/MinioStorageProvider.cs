using Dukaan.Media.Application.Interfaces;
using ErrorOr;
using Minio;
using Minio.DataModel.Args;

namespace Dukaan.Media.Infrastructure.Storage;

public class MinioStorageProvider(IMinioClient minioClient, string bucketName, string? externalEndpoint = null, bool useSSL = false) : IStorageProvider
{
    public async Task<ErrorOr<string>> UploadAsync(Stream stream, string key, string contentType)
    {
        try
        {
            await EnsureBucketExistsAsync().ConfigureAwait(false);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return key;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.UploadFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<string>> UploadChunkAsync(Stream stream, string key, string contentType)
    {
        try
        {
            await EnsureBucketExistsAsync().ConfigureAwait(false);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            return key;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.UploadChunkFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(string key)
    {
        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithCallbackStream(async (s, ct) => await s.CopyToAsync(memoryStream, ct).ConfigureAwait(false));

            await minioClient.GetObjectAsync(args).ConfigureAwait(false);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.DownloadFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Stream>> DownloadChunkAsync(string key)
    {
        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithCallbackStream(async (s, ct) => await s.CopyToAsync(memoryStream, ct).ConfigureAwait(false));

            await minioClient.GetObjectAsync(args).ConfigureAwait(false);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.DownloadChunkFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<string>> GetPresignedUrlAsync(string key, TimeSpan expiry)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key)
                .WithExpiry((int)expiry.TotalSeconds);

            string url = await minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(externalEndpoint))
            {
                var scheme = useSSL ? "https" : "http";
                var uri = new Uri(url);
                url = $"{scheme}://{externalEndpoint}{uri.PathAndQuery}";
            }

            return url;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.UrlGenerationFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(string key)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(key);

            await minioClient.RemoveObjectAsync(args).ConfigureAwait(false);
            return Result.Deleted;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.DeleteFailed", ex.Message);
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteChunksAsync(List<string> chunkKeys)
    {
        try
        {
            foreach (var key in chunkKeys)
            {
                var removeArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(key);

                await minioClient.RemoveObjectAsync(removeArgs).ConfigureAwait(false);
            }

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.DeleteChunksFailed", ex.Message);
        }
    }

    public async Task SetBucketPublicReadAsync()
    {
        await EnsureBucketExistsAsync().ConfigureAwait(false);

        var policy = $$"""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {"AWS": ["*"]},
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{bucketName}}/*"]
                }
            ]
        }
        """;

        var args = new SetPolicyArgs()
            .WithBucket(bucketName)
            .WithPolicy(policy);

        await minioClient.SetPolicyAsync(args).ConfigureAwait(false);
    }

    public async Task<ErrorOr<Stream>> CombineChunksAsync(List<string> chunkKeys)
    {
        try
        {
            var combinedStream = new MemoryStream();

            foreach (var chunkKey in chunkKeys.OrderBy(k => k))
            {
                var downloadResult = await DownloadChunkAsync(chunkKey).ConfigureAwait(false);
                if (downloadResult.IsError)
                {
                    return Error.Failure("Storage.CombineChunksFailed", "Failed to download chunk");
                }

                var chunkStream = downloadResult.Value;
                await chunkStream.CopyToAsync(combinedStream).ConfigureAwait(false);
                await chunkStream.DisposeAsync().ConfigureAwait(false);
            }

            combinedStream.Position = 0;
            return combinedStream;
        }
        catch (Exception ex)
        {
            return Error.Failure("Storage.CombineChunksFailed", ex.Message);
        }
    }

    private async Task EnsureBucketExistsAsync()
    {
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }
    }
}
