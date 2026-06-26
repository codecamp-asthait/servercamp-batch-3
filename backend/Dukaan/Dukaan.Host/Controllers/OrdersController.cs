using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Orders.Commands.PlaceOrder;
using Dukaan.Application.Features.Orders.Dtos;
using Dukaan.Application.Features.Orders.Queries.GetOrder;
using Dukaan.Application.Features.Orders.Queries.GetOrders;
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

    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderSummaryDto>>> GetOrders(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(
            await Mediator.Send(new GetOrdersQuery(pageNumber, pageSize)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new GetOrderQuery(id)));
    }
}
