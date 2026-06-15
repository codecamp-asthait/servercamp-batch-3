using Dukaan.Media.Application.Dtos;
using ErrorOr;
using MediatR;

namespace Dukaan.Media.Application.Features.GetMedia;

public record GetMediaQuery(Guid MediaId) : IRequest<ErrorOr<MediaMetadataResponse>>;
