using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;

public record GetCategoriesByParentQuery(Guid ParentCategoryId) : IQuery<IEnumerable<CategoryDto>>;
