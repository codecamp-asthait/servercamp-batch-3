using Dukaan.Application.Features.Customers.Commands.RegisterCustomer;
using Dukaan.Application.Features.Customers.Commands.UpdateCustomerProfile;
using Dukaan.Application.Features.Customers.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Features.Customers.Queries.GetCustomerProfile;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/[controller]")]
public class CustomersController(
    ITenantProvider tenantProvider) : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<CustomerDto>> Register(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        RegisterCustomerCommand command)
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantId.Value!.Value);
        
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<Guid?>> GetCurrentId(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantId.Value!.Value);
        
        return ToActionResult(await Mediator.Send(new GetCurrentCustomerIdQuery()));
    }

    [HttpGet("me/profile")]
    [Authorize]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantId.Value!.Value);

        return ToActionResult(await Mediator.Send(new GetCustomerProfileQuery()));
    }

    [HttpPut("me/profile")]
    [Authorize]
    public async Task<ActionResult<CustomerProfileDto>> UpdateProfile(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        UpdateCustomerProfileCommand command)
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();
        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantId.IsError) return NotFound();
        tenantProvider.SetTenantId(tenantId.Value!.Value);

        return ToActionResult(await Mediator.Send(command));
    }
}
