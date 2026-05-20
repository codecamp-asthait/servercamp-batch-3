using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Controller for managing product categories within a tenant's store.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paged list of root categories for the current tenant.
    /// </summary>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>A paged response containing the list of root categories.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetAll([FromQuery] PaginationRequest request)
    => Ok(await categoryService.GetAllAsync(request));

    /// <summary>
    /// Retrieves a specific category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>The category details if found; otherwise, NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> Get(Guid id)
    {
        var category = await categoryService.GetByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// Creates a new category for the current tenant.
    /// </summary>
    /// <param name="request">The category details.</param>
    /// <returns>The created category details.</returns>
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(CategoryRequestDto request)
        => Ok(await categoryService.CreateAsync(request));

    /// <summary>
    /// Updates an existing category's details.
    /// </summary>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="request">The updated category details.</param>
    /// <returns>NoContent if successful; NotFound if the category does not exist.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CategoryRequestDto request)
        => await categoryService.UpdateAsync(id, request) ? NoContent() : NotFound();

    /// <summary>
    /// Deletes a category (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>NoContent if successful; NotFound if the category does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await categoryService.DeleteAsync(id) ? NoContent() : NotFound();
}