using Dukaan.Media.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.UploadMedia;

public record UploadChunkCommand(
    Guid MediaId,
    int ChunkIndex,
    Stream ChunkStream,
    long ChunkLength,
    string ContentType) : IRequest<ErrorOr<UploadChunkResponse>>;
