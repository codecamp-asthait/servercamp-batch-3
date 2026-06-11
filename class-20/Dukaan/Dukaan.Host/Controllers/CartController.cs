using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Controller for managing the customer's shopping cart.
/// </summary>
[Authorize] // Should be restricted by policy in a real app, keeping it simple for now
[ApiController]
[Route("api/[controller]")]
public class CartController(
    ICartService cartService,
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

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await cartService.GetCartAsync());
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponseDto>> AddItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromBody] AddToCartRequestDto request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        try
        {
            return Ok(await cartService.AddItemAsync(request));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartResponseDto>> UpdateQuantity(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId,
        [FromBody] UpdateQuantityRequestDto request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        try
        {
            return Ok(await cartService.UpdateQuantityAsync(productId, request));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartResponseDto>> RemoveItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await cartService.RemoveItemAsync(productId));
    }

    [HttpDelete]
    public async Task<ActionResult<CartResponseDto>> ClearCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await cartService.ClearCartAsync());
    }
}
