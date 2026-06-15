namespace Dukaan.Media.Domain.Entities;

public class MediaChunk
{
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }
    public MediaMetadata Media { get; set; } = null!;
    public int ChunkIndex { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public long ChunkSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
