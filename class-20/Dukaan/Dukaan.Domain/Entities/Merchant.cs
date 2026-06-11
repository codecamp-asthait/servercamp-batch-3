
using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

public class Merchant : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationUserId { get; set; }
    public Guid TenantId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}
