using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Media.Dtos;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Queries.GetMedia;

public record GetMediaQuery(Guid MediaId) : IQuery<ErrorOr<MediaMetadataResponse>>;
