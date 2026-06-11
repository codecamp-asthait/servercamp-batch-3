using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Features.Products.Queries.GetActiveProducts;
using Dukaan.Application.Features.Products.Queries.GetActiveProductsByCategory;
using Dukaan.Application.Features.Products.Queries.GetProductById;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Application.Dtos;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/storefront")]
public class StorefrontController(
    ITenantProvider tenantProvider) : BaseApiController
{
    [HttpGet("{slug}/products")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProducts(
        string slug,
        [FromQuery] PaginationRequest request)
    {
        var tenantResult = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantResult.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantResult.Value!.Value);
        
        return ToActionResult(await Mediator.Send(new GetActiveProductsQuery(request)));
    }

    [HttpGet("{slug}/products/{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(string slug, Guid id)
    {
        var tenantResult = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantResult.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantResult.Value!.Value);
        
        return ToActionResult(await Mediator.Send(new GetProductByIdQuery(id)));
    }

    [HttpGet("{slug}/categories/{categoryId}/products")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetProductsByCategory(
        string slug,
        Guid categoryId,
        [FromQuery] PaginationRequest request)
    {
        var tenantResult = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantResult.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantResult.Value!.Value);
        
        return ToActionResult(await Mediator.Send(new GetActiveProductsByCategoryQuery(categoryId, request)));
    }
}
