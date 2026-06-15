using Dukaan.Media.Domain.Enums;
using Dukaan.Media.Domain.Interfaces;

namespace Dukaan.Media.Domain.Entities;

public class MediaMetadata : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StagingKey { get; set; } = string.Empty;
    public MediaStatus Status { get; set; } = MediaStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<MediaVariant> Variants { get; set; } = [];
}
