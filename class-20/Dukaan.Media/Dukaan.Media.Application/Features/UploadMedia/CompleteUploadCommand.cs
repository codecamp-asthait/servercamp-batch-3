using Dukaan.Media.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.UploadMedia;

public record CompleteUploadCommand(Guid MediaId) : IRequest<ErrorOr<CompleteUploadResponse>>;
