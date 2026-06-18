using Dukaan.Domain.Entities;
using Dukaan.Application.Interfaces;
using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesDropdown;

public class GetCategoriesDropdownQueryHandler(IRepository<Category> repository)
    : IQueryHandler<GetCategoriesDropdownQuery, ErrorOr<IEnumerable<CategoryDropdownDto>>>
{
    public async Task<ErrorOr<IEnumerable<CategoryDropdownDto>>> Handle(
        GetCategoriesDropdownQuery request, CancellationToken cancellationToken)
    {
        var all = (await repository
            .FindAsync(c => c.IsActive, false, cancellationToken))
            .ToList();
        
        var map = all.ToDictionary(c => c.Id);

        return all
            .Select(c => new CategoryDropdownDto(c.Id, BuildLabel(c), c.Description))
            .ToList();

        string BuildLabel(Category c)
        {
            var segments = new List<string>();
            var current = c;
            while (current != null)
            {
                segments.Add(current.Name);
                current = current.ParentCategoryId.HasValue
                          && map.TryGetValue(current.ParentCategoryId.Value, out var parent) ? parent : null;
            }
            segments.Reverse();
            return string.Join(" > ", segments);
        }
    }
}