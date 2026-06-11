using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

public record GetTenantIdFromSlugQuery(string Slug) : IQuery<ErrorOr<Guid?>>;
