using Dukaan.Application.Features.Addresses.Commands.CreateAddress;
using Dukaan.Application.Features.Addresses.Commands.DeleteAddress;
using Dukaan.Application.Features.Addresses.Commands.SetDefaultAddress;
using Dukaan.Application.Features.Addresses.Commands.UpdateAddress;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Application.Features.Addresses.Queries.GetAddressById;
using Dukaan.Application.Features.Addresses.Queries.GetCustomerAddresses;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class AddressesController(
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
    public async Task<ActionResult<AddressDto>> Create(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        CreateAddressCommand command)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AddressDto>> Update(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid id,
        [FromBody] UpdateAddressData data)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new UpdateAddressCommand(id, data)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new DeleteAddressCommand(id)));
    }

    [HttpPatch("{id}/set-default")]
    public async Task<ActionResult<AddressDto>> SetDefault(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new SetDefaultAddressCommand(id)));
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetAll(
        [FromHeader(Name = "x-tenant-slug")] string? slug)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new GetCustomerAddressesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetById(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        Guid id)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(new GetAddressByIdQuery(id)));
    }
}
