using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;

namespace Dukaan.Host.Controllers;

[ApiController]
[Route("api/storefront")]
public class StorefrontController(
    IProductService productService,
    ICategoryService categoryService,
    ITenantService tenantService,
    ITenantProvider tenantProvider) : ControllerBase
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantId = await tenantService.GetTenantIdFromSlug(slug);
        if (tenantId is null) return false;
        tenantProvider.SetTenantId(tenantId.Value);
        return true;
    }

    [HttpGet("products")]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetProducts(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await productService.GetActiveAsync(request));
    }

    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(
        [FromHeader(Name = "x-tenant-slug")] string? slug, Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        var product = await productService.GetByIdAsync(id);
        return product is null or { IsActive: false } ? NotFound() : Ok(product);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetCategories(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await categoryService.GetAllAsync(request));
    }

    [HttpGet("categories/{categoryId}/products")]
    public async Task<ActionResult<PagedResponse<ProductResponseDto>>> GetProductsByCategory(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid categoryId, [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await productService.GetActiveByCategoryAsync(categoryId, request));
    }
}
