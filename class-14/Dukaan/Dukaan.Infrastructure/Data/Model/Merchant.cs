using Dukaan.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Infrastructure.Data.Model;

/// <summary>
/// Represents a user (Merchant) in the system who owns or operates a store.
/// </summary>
/// <remarks>
/// It extends <see cref="IdentityUser{Guid}"/> to leverage ASP.NET Core Identity's 
/// authentication and authorization features. 
/// Each Merchant is linked to a specific <see cref="Dukaan.Domain.Entities.Tenant"/>.
/// </remarks>
public class Merchant : IdentityUser<Guid>, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the Tenant this merchant belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The date and time when the merchant was registered.
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}