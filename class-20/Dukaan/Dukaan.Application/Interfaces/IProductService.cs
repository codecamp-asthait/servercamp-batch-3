using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// Interface for product management services.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Asynchronously retrieves a paged list of all products for the current tenant.
    /// </summary>
    /// <param name="request">The pagination and filtering request.</param>
    /// <returns>A paged response containing the list of products.</returns>
    Task<PagedResponse<ProductResponseDto>> GetAllAsync(PaginationRequest request);

    Task<PagedResponse<ProductResponseDto>> GetActiveAsync(PaginationRequest request);

    /// <summary>
    /// Asynchronously creates a new product.
    /// </summary>
    /// <param name="request">The product creation request data.</param>
    /// <returns>A task representing the asynchronous operation, containing the created product response.</returns>
    Task<ProductResponseDto> CreateAsync(ProductRequestDto request);

    /// <summary>
    /// Asynchronously retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product DTO if found; otherwise, null.</returns>
    Task<ProductResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="request">The updated product details.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    Task<bool> UpdateAsync(Guid id, ProductRequestDto request);

    /// <summary>
    /// Asynchronously deletes (soft delete) a product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously attaches a category to a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>True if the attachment was successful; otherwise, false.</returns>
    Task<bool> AttachCategoryAsync(Guid productId, Guid categoryId);

    /// <summary>
    /// Asynchronously detaches a category from a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>True if the detachment was successful; otherwise, false.</returns>
    Task<bool> DetachCategoryAsync(Guid productId, Guid categoryId);

    Task<PagedResponse<ProductResponseDto>> GetActiveByCategoryAsync(Guid categoryId, PaginationRequest request);

    /// <summary>
    /// Asynchronously retrieves a paged list of products belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paged response containing the list of products in the category.</returns>
    Task<PagedResponse<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, PaginationRequest request);
}