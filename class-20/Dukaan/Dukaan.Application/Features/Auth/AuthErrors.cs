using ErrorOr;

namespace Dukaan.Application.Features.Auth;

public static class AuthErrors
{
    public static Error InvalidCredentials => Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
    public static Error EmailAlreadyRegistered => Error.Conflict("Auth.EmailAlreadyRegistered", "Email is already registered.");
    public static Error IdentityCreationFailed => Error.Unexpected("Auth.IdentityCreationFailed", "Failed to create user identity.");
}
