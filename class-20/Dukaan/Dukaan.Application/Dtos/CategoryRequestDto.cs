namespace Dukaan.Application.Dtos;

/// <summary>
/// Data transfer object for creating or updating a category.
/// </summary>
/// <param name="Name">The name of the category.</param>
/// <param name="Description">The optional description of the category.</param>
/// <param name="ParentCategoryId">The optional unique identifier of the parent category.</param>
public record CategoryRequestDto(string Name, string? Description, Guid? ParentCategoryId);

/// <summary>
/// Data transfer object for category response.
/// </summary>
/// <param name="Id">The unique identifier of the category.</param>
/// <param name="Name">The name of the category.</param>
/// <param name="Description">The description of the category.</param>
/// <param name="ParentCategoryId">The unique identifier of the parent category, if any.</param>
/// <param name="SubCategories">The list of sub-categories belonging to this category.</param>
public record CategoryResponseDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    List<CategoryResponseDto> SubCategories
);
