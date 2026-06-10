using Dukaan.Domain.Entities;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;

namespace Dukaan.Application.Services;

/// <summary>
/// Service implementation for managing products.
/// </summary>
/// <param name="productRepository">The generic repository for Product entities.</param>
/// <param name="categorizedProductRepository">The generic repository for CategorizedProduct entities.</param>
public class ProductService(IRepository<Product> productRepository, IRepository<CategorizedProduct> categorizedProductRepository) : IProductService
{
    /// <summary>
    /// Asynchronously retrieves a paged list of all products for the current tenant.
    /// </summary>
    /// <param name="request">The pagination and filtering request.</param>
    /// <returns>A paged response containing the list of products.</returns>
    public async Task<PagedResponse<ProductResponseDto>> GetAllAsync(PaginationRequest request)
    {
        var (items, totalCount) = await productRepository.GetPagedAsync(request.PageNumber, request.PageSize);

        var dtos = items.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.StockQuantity,
            p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList()
        ));

        return new PagedResponse<ProductResponseDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }


    /// <summary>
    /// Asynchronously retrieves a paged list of active products for the current tenant.
    /// </summary>
    /// <param name="request">The pagination and filtering request.</param>
    /// <returns>A paged response containing the list of active products.</returns>
    public async Task<PagedResponse<ProductResponseDto>> GetActiveAsync(PaginationRequest request)
    {
        var (items, totalCount) = await productRepository.GetPagedAsync(
            p => p.IsActive, request.PageNumber, request.PageSize, trackChanges: false,
            p => p.ProductCategories);
        return new PagedResponse<ProductResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
    }

    /// <summary>
    /// Asynchronously retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product DTO if found; otherwise, null.</returns>
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var p = await productRepository.GetByIdAsync(id, trackChanges: false);
        return p == null
            ? null
            : new ProductResponseDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive,
                p.ProductCategories.Select(pc => pc.CategoryId).ToList());
    }

    /// <summary>
    /// Creates a new product and persists it to the database.
    /// </summary>
    /// <param name="request">The product details.</param>
    /// <returns>A DTO representing the created product.</returns>
    public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            StockQuantity = request.StockQuantity
        };

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        return new ProductResponseDto(product.Id, product.Name, product.Description,
            product.Price, product.ImageUrl, product.StockQuantity, product.IsActive, []);
    }

    /// <summary>
    /// Asynchronously updates an existing product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="request">The updated product details.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public async Task<bool> UpdateAsync(Guid id, ProductRequestDto request)
    {
        var product = await productRepository.GetByIdAsync(id, trackChanges: true);
        if (product == null) return false;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.StockQuantity = request.StockQuantity;

        productRepository.Update(product);
        await productRepository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously deletes (soft delete) a product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id, trackChanges: true);
        if (product == null) return false;

        product.IsActive = false;
        productRepository.Update(product);
        await productRepository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously attaches a category to a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>True if the attachment was successful; otherwise, false.</returns>
    public async Task<bool> AttachCategoryAsync(Guid productId, Guid categoryId)
    {
        await categorizedProductRepository.AddAsync(new CategorizedProduct
        {
            ProductId = productId,
            CategoryId = categoryId
        });

        await categorizedProductRepository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Asynchronously detaches a category from a product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>True if the detachment was successful; otherwise, false.</returns>
    public async Task<bool> DetachCategoryAsync(Guid productId, Guid categoryId)
    {
        var associations =
            await categorizedProductRepository.FindAsync(cp =>
                cp.ProductId == productId && cp.CategoryId == categoryId);

        var association = associations.FirstOrDefault();
        if (association == null) return false;

        categorizedProductRepository.Remove(association);
        await categorizedProductRepository.SaveChangesAsync();
        return true;
    }


    /// <summary>
    /// Asynchronously retrieves a paged list of active products belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paged response containing the list of active products in the category.</returns>
    public async Task<PagedResponse<ProductResponseDto>> GetActiveByCategoryAsync(Guid categoryId, PaginationRequest request)
    {
        var (items, totalCount) = await productRepository.GetPagedAsync(
            p => p.IsActive && p.ProductCategories.Any(pc => pc.CategoryId == categoryId),
            request.PageNumber, request.PageSize, trackChanges: false,
            p => p.ProductCategories);
        return new PagedResponse<ProductResponseDto>(items.Select(MapToDto), totalCount, request.PageNumber, request.PageSize);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of products belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paged response containing the list of products in the category.</returns>
    public async Task<PagedResponse<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, PaginationRequest request)
    {
        var (items, totalCount) = await productRepository.GetPagedAsync(
            p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId),
            request.PageNumber,
            request.PageSize,
            trackChanges: false,
            p => p.ProductCategories);

        var dtos = items.Select(p => new ProductResponseDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.ImageUrl,
            p.StockQuantity,
            p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList()));

        return new PagedResponse<ProductResponseDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }

    private static ProductResponseDto MapToDto(Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.IsActive,
            p.ProductCategories.Select(pc => pc.CategoryId).ToList());

}