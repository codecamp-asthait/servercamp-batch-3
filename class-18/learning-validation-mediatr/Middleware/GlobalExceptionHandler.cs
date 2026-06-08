using FluentValidation;

namespace learning_validation_mediatr.Middleware;

/// <summary>
/// Global exception handler middleware. Catches <see cref="ValidationException"/>
/// thrown by the MediatR pipeline and returns a structured 400 response.
/// All other unhandled exceptions return 500.
/// </summary>
public class GlobalExceptionHandler(RequestDelegate next)
{
    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { errors });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}
