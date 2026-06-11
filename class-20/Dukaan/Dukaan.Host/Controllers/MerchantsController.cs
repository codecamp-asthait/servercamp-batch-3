using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Merchants.Commands.RegisterMerchant;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Route("api/[controller]")]
public class MerchantsController : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(MerchantRegisterRequest request)
    {
        var response = await Mediator.Send(new RegisterMerchantCommand(
            request.Email,
            request.PhoneNumber,
            request.Password,
            request.StoreName,
            request.Slug,
            request.Category,
            request.Country));
        return Ok(response);
    }
}
