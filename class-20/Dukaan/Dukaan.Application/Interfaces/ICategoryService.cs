using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// Interface for category management services.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Asynchronously retrieves a paged list of all categories for the current tenant.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paged response containing the list of categories.</returns>
    Task<PagedResponse<CategoryResponseDto>> GetAllAsync(PaginationRequest request);

    /// <summary>
    /// Asynchronously retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The category DTO if found; otherwise, null.</returns>
    Task<CategoryResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously creates a new category.
    /// </summary>
    /// <param name="request">The category creation request data.</param>
    /// <returns>The created category details.</returns>
    Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request);

    /// <summary>
    /// Asynchronously updates an existing category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="request">The updated category details.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    Task<bool> UpdateAsync(Guid id, CategoryRequestDto request);

    /// <summary>
    /// Asynchronously deletes a category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);
}