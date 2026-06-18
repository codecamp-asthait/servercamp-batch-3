using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public record CheckSlugAvailabilityQuery(string Slug) : IQuery<ErrorOr<bool>>;
