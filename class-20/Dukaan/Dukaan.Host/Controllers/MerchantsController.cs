using Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;
using Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;
using Dukaan.Application.Features.Tenants.Queries.GetTenantIdFromSlug;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class MerchantsController(ITenantProvider tenantProvider) : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<MerchantDto>> Register(RegisterMerchantCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsError && result.Value is not null)
        {
            var tenantId = await Mediator.Send(new GetTenantIdFromSlugQuery(result.Value.Slug));
            if (!tenantId.IsError && tenantId.Value is not null)
            {
                tenantProvider.SetTenantId(tenantId.Value.Value);
            }
        }
        return ToActionResult(result);
    }

    [HttpGet("profile")]
    public async Task<ActionResult<MerchantDto>> GetProfile()
        => ToActionResult(await Mediator.Send(new GetMerchantProfileQuery()));

    [HttpGet("check-slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CheckSlug(string slug)
        => ToActionResult(await Mediator.Send(new CheckSlugAvailabilityQuery(slug)));
}
