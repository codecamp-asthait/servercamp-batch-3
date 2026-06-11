using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Features.Categories.Queries.GetCategories;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Features.Products.Queries.GetActiveProducts;
using Dukaan.Application.Features.Products.Queries.GetActiveProductsByCategory;
using Dukaan.Application.Features.Products.Queries.GetProductById;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/storefront")]
public class StorefrontController(ITenantProvider tenantProvider) : BaseApiController
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId is null) return false;
        tenantProvider.SetTenantId(tenantId.Value);
        return true;
    }

    [HttpGet("products")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new GetActiveProductsQuery(request)));
    }

    [HttpGet("products/{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(
        [FromHeader(Name = "x-tenant-slug")] string? slug, Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        var product = await Mediator.Send(new GetProductByIdQuery(id));
        return product is null or { IsActive: false } ? NotFound() : Ok(product);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<PagedResponse<CategoryDto>>> GetCategories(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new GetCategoriesQuery(request)));
    }

    [HttpGet("categories/{categoryId}/products")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProductsByCategory(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid categoryId, [FromQuery] PaginationRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new GetActiveProductsByCategoryQuery(categoryId, request)));
    }
}
