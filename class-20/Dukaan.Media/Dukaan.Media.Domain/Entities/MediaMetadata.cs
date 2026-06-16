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
    public string? ImagePath { get; set; }
    public MediaStatus Status { get; set; } = MediaStatus.Uploading;
    
    // Chunking fields
    public int TotalChunks { get; set; }
    public long ChunkSize { get; set; }
    public long TotalFileSize { get; set; }
    public int UploadedChunks { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<MediaChunk> Chunks { get; set; } = [];
    public ICollection<MediaVariant> Variants { get; set; } = [];
}
