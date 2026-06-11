using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

public record GetTenantIdFromSlugQuery(string Slug) : IQuery<Guid?>;
