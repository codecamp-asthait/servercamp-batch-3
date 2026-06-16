using Dukaan.Media.Application.Core.Abstractions;
using Dukaan.Media.Application.Features.Media.Dtos;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Queries.GetMediaUrl;

public record GetMediaUrlQuery(Guid MediaId, string Variant = "display") : IQuery<ErrorOr<MediaUrlResponse>>;
