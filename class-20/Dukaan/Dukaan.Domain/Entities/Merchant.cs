
using Dukaan.Domain.Interfaces;

namespace dukaan.Domain.Entities;

public class Merchant : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public Guid TenantId { get; set; }
}
