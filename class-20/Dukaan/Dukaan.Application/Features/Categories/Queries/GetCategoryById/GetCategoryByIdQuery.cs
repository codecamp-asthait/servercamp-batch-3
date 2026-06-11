using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IQuery<ErrorOr<CategoryDto?>>;
