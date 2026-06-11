using Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;
using Dukaan.Application.Features.Merchants.Dtos;
using Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;
using Dukaan.Application.Features.Merchants.Queries.GetMerchantProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class MerchantsController : BaseApiController
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<MerchantDto>> Register(RegisterMerchantCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpGet("profile")]
    public async Task<ActionResult<MerchantDto>> GetProfile()
        => ToActionResult(await Mediator.Send(new GetMerchantProfileQuery()));

    [HttpGet("check-slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CheckSlug(string slug)
        => ToActionResult(await Mediator.Send(new CheckSlugAvailabilityQuery(slug)));
}
