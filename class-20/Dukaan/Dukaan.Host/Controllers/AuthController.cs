using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Features.Auth.Dtos;
using Dukaan.Application.Features.Auth.Commands.Login;
using Dukaan.Application.Features.Auth.Commands.CustomerLogin;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;

namespace Dukaan.Host.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginWithJwt(LoginCommand command)
    {
        try
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("customer/login")]
    public async Task<ActionResult<CustomerAuthResponse>> CustomerLogin(
        [FromHeader(Name = "x-tenant-slug")] string tenantSlug,
        CustomerLoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(tenantSlug)) return BadRequest("Store not found.");

        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(tenantSlug));
        if (tenantId is null) return NotFound("Store not found.");

        var response = await Mediator.Send(command with { TenantId = tenantId.Value });
        return response == null ? Unauthorized() : Ok(response);
    }
}
