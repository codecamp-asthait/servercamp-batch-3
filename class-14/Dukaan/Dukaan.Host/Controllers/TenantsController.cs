using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Infrastructure.Services;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Controller for managing tenant and merchant registrations.
/// </summary>
/// <remarks>
/// This controller serves as the entry point for tenant-related API calls.
/// It delegates business logic to the <see cref="TenantService"/>.
/// </remarks>
[ApiController]
[Route("api/[controller]")] // domain/api/tenants
public class TenantsController(TenantService tenantService) : ControllerBase
{
    /// <summary>
    /// Endpoint for registering a new merchant and store.
    /// </summary>
    /// <param name="request">The registration data.</param>
    /// <returns>The registration response.</returns>
    /// <response code="200">Returns the newly created tenant details.</response>
    [HttpPost("register")] // domain/api/tenants/register
    public async Task<ActionResult> Register(MerchantRegisterRequest request)
    {
        var response = await tenantService.RegisterMerchant(request);
        return Ok(response);
    }
}