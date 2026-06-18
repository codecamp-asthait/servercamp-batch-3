using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads.Commands.UploadChunk;

public record UploadChunkCommand(
    Guid MediaId,
    int ChunkIndex,
    Stream ChunkStream,
    long ChunkLength,
    string ContentType) : ICommand<ErrorOr<UploadChunkResponse>>;
