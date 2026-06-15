using Dukaan.Media.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.UploadMedia;

public record InitiateUploadCommand(
    string FileName,
    string ContentType,
    long TotalFileSize) : IRequest<ErrorOr<InitiateUploadResponse>>;
