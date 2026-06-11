using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/[controller]")]
public class MerchantsController : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterMerchantCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}
