
using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

public class Customer : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = [];
}
