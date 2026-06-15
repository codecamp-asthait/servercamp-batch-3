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

public record UpdateQuantityRequest(int Quantity);

[Authorize]
public class CartController(
    ITenantProvider tenantProvider) : BaseApiController
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantResult = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantResult.IsError) return false;
        tenantProvider.SetTenantId(tenantResult.Value!.Value);
        return true;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new GetCartQuery()));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromBody] AddToCartCommand command)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartDto>> UpdateQuantity(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId,
        [FromBody] UpdateQuantityRequest request)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new UpdateCartItemQuantityCommand(productId, request.Quantity)));
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid productId)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new RemoveCartItemCommand(productId)));
    }

    [HttpDelete]
    public async Task<ActionResult<CartDto>> ClearCart(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new ClearCartCommand()));
    }
}
