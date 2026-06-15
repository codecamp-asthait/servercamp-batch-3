namespace Dukaan.Media.Domain.Entities;

public class MediaVariant
{
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }
    public MediaMetadata Media { get; set; } = null!;
    public string VariantType { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
}
