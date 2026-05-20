using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Controller for managing products within a tenant's store.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paged list of all products for the current tenant.
    /// </summary>
    /// <param name="request">The pagination and filtering request parameters.</param>
    /// <returns>A paged response containing the list of products.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetAll(
        [FromQuery] PaginationRequest request)
    {
        var result = await productService.GetAllAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a paged list of products belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>A paged response containing the list of products in the category.</returns>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetByCategory(Guid categoryId,
        [FromQuery] PaginationRequest request)
        => Ok(await productService.GetByCategoryAsync(categoryId, request));

    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product details if found; otherwise, NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> Get(Guid id)
    {
        var product = await productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Creates a new product for the current tenant.
    /// </summary>
    /// <param name="request">The product details.</param>
    /// <returns>The created product details.</returns>
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(ProductRequestDto request)
    {
        var result = await productService.CreateAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing product's details.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="request">The updated product details.</param>
    /// <returns>NoContent if successful; NotFound if the product does not exist.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ProductRequestDto request)
        => await productService.UpdateAsync(id, request) ? NoContent() : NotFound();

    /// <summary>
    /// Deletes a product (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns>NoContent if successful; NotFound if the product does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) => await productService.DeleteAsync(id) ? NoContent() : NotFound();

    /// <summary>
    /// Associates a category with a product.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>NoContent if successful; NotFound if the association could not be created.</returns>
    [HttpPost("{id}/categories/{categoryId}")]
    public async Task<IActionResult> AttachCategory(Guid id, Guid categoryId)
        => await productService.AttachCategoryAsync(id, categoryId) ? NoContent() : NotFound();

    /// <summary>
    /// Removes the association between a product and a category.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>NoContent if successful; NotFound if the association does not exist.</returns>
    [HttpDelete("{id}/categories/{categoryId}")]
    public async Task<IActionResult> DetachCategory(Guid id, Guid categoryId)
        => await productService.DetachCategoryAsync(id, categoryId) ? NoContent() : NotFound();
}