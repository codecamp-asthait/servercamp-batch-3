using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// Defines methods for authenticating users and managing authentication operations.
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
}