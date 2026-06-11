using Dukaan.Application.DTOs;
using Dukaan.Application.Services;
using Dukaan.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(
    ICustomerService customerService,
    ITenantService tenantService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<CustomerAuthResponse>> Register(
         [FromHeader(Name = "x-tenant-slug")] string tenantSlug,
         CustomerRegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(tenantSlug)) return BadRequest("Store not found.");
            
        var tenantId = await tenantService.GetTenantIdFromSlug(tenantSlug);
        if (tenantId is null) return NotFound("Store not found.");

        try
        {
            var customerId = await customerService.RegisterAsync(request, tenantId.Value);
            return Created(string.Empty, new { customerId });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already registered"))
        {
            return Conflict(ex.Message);
        }
    }
}
