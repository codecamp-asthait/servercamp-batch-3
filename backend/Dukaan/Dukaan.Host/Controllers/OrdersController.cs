using Dukaan.Application.Features.Orders.Commands.PlaceOrder;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class OrdersController(
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

    [HttpPost]
    public async Task<ActionResult<OrderDto>> PlaceOrder(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        PlaceOrderCommand command)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(command));
    }
}
