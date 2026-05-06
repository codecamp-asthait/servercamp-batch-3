using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Dukaan.Application.Interfaces;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Handles authentication-related HTTP requests, including user login operations using JWT authentication.
/// </summary>
/// <remarks>This controller provides endpoints for user authentication workflows. It is intended to be used as
/// part of an API that issues JWT tokens for authenticated clients.</remarks>
/// <param name="authService">The authentication service used to process login requests and generate authentication tokens.</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    /// <summary>
    /// Authenticates a user using the provided credentials and returns a JWT token if authentication is successful.
    /// </summary>
    /// <remarks>Use this endpoint to obtain a JWT token for subsequent authenticated requests. The request
    /// must include valid user credentials. The response format and status codes follow standard authentication
    /// practices.</remarks>
    /// <param name="request">The login credentials and related information required to authenticate the user.</param>
    /// <returns>An IActionResult containing the JWT token and user information if authentication succeeds; otherwise, an
    /// unauthorized result if authentication fails.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginWithJwt(LoginRequestDTO request)   // domain/api/auth/login
    {
        try
        {
            var response = await authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)  // Catch the UnauthorizedAccessException from the service layer and return a 401 Unauthorized response to the client
        {
            return Unauthorized(ex.Message);
        }
    }
}