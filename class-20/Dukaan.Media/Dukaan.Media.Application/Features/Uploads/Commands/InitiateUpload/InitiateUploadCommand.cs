using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads.Commands.InitiateUpload;

public record InitiateUploadCommand(
    string FileName,
    string ContentType,
    long TotalFileSize) : ICommand<ErrorOr<InitiateUploadResponse>>;
