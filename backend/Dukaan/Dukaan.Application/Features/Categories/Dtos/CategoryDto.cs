namespace Dukaan.Application.Features.Categories.Dtos;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    List<CategoryDto> SubCategories
);

public record CategoryDropdownDto(Guid Id, string Name, string? Description);