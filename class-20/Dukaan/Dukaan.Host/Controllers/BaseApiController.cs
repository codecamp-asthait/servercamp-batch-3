using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IMediator Mediator => 
        HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected ActionResult<T> ToActionResult<T>(ErrorOr<T> result)
    {
        return result.Match(
            value => Ok(value),
            errors => Problem(errors)
        );
    }

    protected ActionResult ToActionResult(ErrorOr<Success> result)
    {
        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    protected ActionResult ToActionResult(ErrorOr<Deleted> result)
    {
        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    private ActionResult Problem(List<Error> errors)
    {
        var firstError = errors.First();
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            statusCode: statusCode,
            title: firstError.Code,
            detail: firstError.Description
        );
    }
}
