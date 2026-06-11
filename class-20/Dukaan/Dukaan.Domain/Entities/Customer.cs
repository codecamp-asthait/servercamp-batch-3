
using Dukaan.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Dukaan.Domain.Entities;

public class Customer : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public IdentityUser<Guid>? ApplicationUser { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
