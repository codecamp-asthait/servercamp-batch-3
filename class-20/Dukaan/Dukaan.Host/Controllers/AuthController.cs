using Dukaan.Application.Features.Auth.Commands.CustomerLogin;
using Dukaan.Application.Features.Auth.Commands.Login;
using Dukaan.Application.Features.Auth.Commands.RegisterUser;
using Dukaan.Application.Features.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthDto>> Login(LoginCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPost("customer-login")]
    [AllowAnonymous]
    public async Task<ActionResult<CustomerAuthDto>> CustomerLogin(CustomerLoginCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthDto>> Register(RegisterUserCommand command)
        => ToActionResult(await Mediator.Send(command));
}
