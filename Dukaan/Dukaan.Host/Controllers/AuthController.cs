using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Infrastructure.Services.Interfaces;
using Dukaan.Application.Features.Auth.Commands.Login;
using Dukaan.Application.Features.Auth.Commands.CustomerLogin;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

namespace Dukaan.Host.Controllers;

public class AuthController(ITenantProvider tenantProvider) : BaseApiController
{
    private async Task<bool> ResolveTenant(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var tenantResult = await Mediator.Send(new GetTenantIdFromSlugQuery(slug));
        if (tenantResult.IsError) return false;
        tenantProvider.SetTenantId(tenantResult.Value!.Value);
        return true;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthDto>> Login(LoginCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPost("customer/login")]
    [AllowAnonymous]
    public async Task<ActionResult<CustomerAuthDto>> CustomerLogin(
        [FromHeader(Name = "x-tenant-slug")] string? slug,
        CustomerLoginCommand command)
    {
        if (!await ResolveTenant(slug)) return NotFound();
        return ToActionResult(await Mediator.Send(command));
    }
}
