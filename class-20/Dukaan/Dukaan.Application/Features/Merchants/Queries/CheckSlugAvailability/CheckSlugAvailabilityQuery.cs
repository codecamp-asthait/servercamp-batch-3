using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public record CheckSlugAvailabilityQuery(string Slug) : IQuery<bool>;
