using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Uploads.Dtos;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Uploads.Commands.CompleteUpload;

public record CompleteUploadCommand(Guid MediaId) : ICommand<ErrorOr<CompleteUploadResponse>>;
