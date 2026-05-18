using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Dukaan.Host.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create(ProductRequestDto request)
    {
        var result = await productService.CreateAsync(request);
        return Ok(result);
    }
}