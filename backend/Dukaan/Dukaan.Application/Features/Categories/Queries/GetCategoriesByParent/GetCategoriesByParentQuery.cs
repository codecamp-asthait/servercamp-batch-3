using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;

public record GetCategoriesByParentQuery(Guid ParentId) : IQuery<ErrorOr<IEnumerable<CategoryDto>>>;
