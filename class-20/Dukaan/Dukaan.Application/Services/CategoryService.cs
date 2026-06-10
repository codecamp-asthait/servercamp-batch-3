using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Services;

/// <summary>
/// Service implementation for managing product categories.
/// </summary>
/// <param name="repository">The generic repository for Category entities.</param>
public class CategoryService(IRepository<Category> repository) : ICategoryService
{
    /// <summary>
    /// Asynchronously retrieves a paged list of root categories for the current tenant.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paged response containing the list of root categories with their sub-categories.</returns>
    public async Task<PagedResponse<CategoryResponseDto>> GetAllAsync(PaginationRequest request)
    {
        // Only paginate root categories to maintain tree structure integrity
        var (items, totalCount) = await repository.GetPagedAsync(
            c => c.ParentCategoryId == null && c.IsActive,
            request.PageNumber,
            request.PageSize,
            false,
            c => c.SubCategories);
        return new PagedResponse<CategoryResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
    }

    /// <summary>
    /// Asynchronously retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The category DTO if found; otherwise, null.</returns>
    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await repository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    /// <summary>
    /// Asynchronously creates a new category.
    /// </summary>
    /// <param name="request">The category creation request data.</param>
    /// <returns>The created category details.</returns>
    /// <exception cref="Exception">Thrown when the specified parent category is not found.</exception>
    public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto request)
    {
        if (request.ParentCategoryId.HasValue)
        {
            _ = await repository.GetByIdAsync(request.ParentCategoryId!.Value)
                ?? throw new Exception("Parent category not found");
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        await repository.AddAsync(category);
        await repository.SaveChangesAsync();

        return MapToDto(category);
    }

    /// <summary>
    /// Asynchronously updates an existing category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="request">The updated category details.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public async Task<bool> UpdateAsync(Guid id, CategoryRequestDto request)
    {
        var category = await repository.GetByIdAsync(id, trackChanges: true);
        if (category == null) return false;

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;

        await repository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously deletes a category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    /// <exception cref="Exception">Thrown when the category has active sub-categories or is assigned to products.</exception>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await repository.GetByIdAsync(id, trackChanges: true);
        if (category == null) return false;

        // Spec requirement: Prevent deletion if it has sub-categories or products
        if (category.SubCategories.Any(sc => sc.IsActive))
            throw new Exception("Cannot delete category with active sub-categories.");
        if (category.ProductLinks.Any()) throw new Exception("Cannot delete category assigned to products.");

        category.IsActive = false;
        await repository.SaveChangesAsync();
        return true;
    }
    
    private CategoryResponseDto MapToDto(Category category) => new(
                category.Id,
                category.Name,
                category.Description,
                category.ParentCategoryId,
                category.SubCategories.Where(s => s.IsActive).Select(MapToDto).ToList());
}
