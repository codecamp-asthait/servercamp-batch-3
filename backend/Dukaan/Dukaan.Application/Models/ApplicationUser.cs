using Dukaan.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Application.Models;

public class ApplicationUser : IdentityUser<Guid>, ITenantEntity
{
    public UserType UserType { get; set; }
    public Guid TenantId { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
