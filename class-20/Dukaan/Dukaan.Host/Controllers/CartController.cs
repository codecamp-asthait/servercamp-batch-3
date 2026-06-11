using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Cart.Commands.AddToCart;
using Dukaan.Application.Features.Cart.Commands.ClearCart;
using Dukaan.Application.Features.Cart.Commands.RemoveCartItem;
using Dukaan.Application.Features.Cart.Commands.UpdateCartItemQuantity;
using Dukaan.Application.Features.Cart.Dtos;
using Dukaan.Application.Features.Cart.Queries.GetCart;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
[Route("api/[controller]")]
public class CartController(
    ITenantProvider tenantProvider) : BaseApiController
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId is null) return false;
        tenantProvider.SetTenantId(tenantId.Value);
        return true;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new GetCartQuery()));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromBody] AddToCartRequestDto request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        try
        {
            return Ok(await Mediator.Send(new AddToCartCommand(request)));
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
    public async Task<ActionResult<CartDto>> UpdateQuantity(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId,
        [FromBody] UpdateQuantityRequestDto request)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        try
        {
            return Ok(await Mediator.Send(new UpdateCartItemQuantityCommand(productId, request)));
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
    public async Task<ActionResult<CartDto>> RemoveItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new RemoveCartItemCommand(productId)));
    }

    [HttpDelete]
    public async Task<ActionResult<CartDto>> ClearCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound("Store not found.");
        return Ok(await Mediator.Send(new ClearCartCommand()));
    }
}
