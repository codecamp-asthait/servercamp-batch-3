using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Customers.Commands.RegisterCustomer;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/[controller]")]
public class CustomersController : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
         [FromHeader(Name = "x-tenant-slug")] string tenantSlug,
         RegisterCustomerCommand command)
    {
        if (string.IsNullOrWhiteSpace(tenantSlug)) return BadRequest("Store not found.");

        var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(tenantSlug));
        if (tenantId is null) return NotFound("Store not found.");

        try
        {
            var customerId = await Mediator.Send(command with { TenantId = tenantId.Value });
            return Created(string.Empty, new { customerId });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already registered"))
        {
            return Conflict(ex.Message);
        }
    }
}
