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
}