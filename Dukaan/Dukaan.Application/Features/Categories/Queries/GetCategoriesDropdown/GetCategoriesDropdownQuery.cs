using ErrorOr;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesDropdown;

public record GetCategoriesDropdownQuery : IQuery<ErrorOr<IEnumerable<CategoryDropdownDto>>>;