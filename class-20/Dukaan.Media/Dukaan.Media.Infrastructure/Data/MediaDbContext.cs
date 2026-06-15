using Dukaan.Media.Application.Interfaces;
using Dukaan.Media.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dukaan.Media.Infrastructure.Data;

public class MediaDbContext(DbContextOptions<MediaDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<MediaMetadata> MediaMetadata { get; set; } = null!;
    public DbSet<MediaChunk> MediaChunks { get; set; } = null!;
    public DbSet<MediaVariant> MediaVariants { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("media");

        modelBuilder.Entity<MediaMetadata>(entity =>
        {
            entity.ToTable("MediaMetadata");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TenantId);
            entity.HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId());
            entity.Property(e => e.OriginalFileName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.StagingKey).HasMaxLength(500);
        });

        modelBuilder.Entity<MediaChunk>(entity =>
        {
            entity.ToTable("MediaChunk");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.MediaId, e.ChunkIndex }).IsUnique();
            entity.HasOne(e => e.Media)
                .WithMany(m => m.Chunks)
                .HasForeignKey(e => e.MediaId);
            entity.Property(e => e.StorageKey).HasMaxLength(500);
        });

        modelBuilder.Entity<MediaVariant>(entity =>
        {
            entity.ToTable("MediaVariant");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Media)
                .WithMany(m => m.Variants)
                .HasForeignKey(e => e.MediaId);
            entity.Property(e => e.VariantType).HasMaxLength(50);
            entity.Property(e => e.StorageKey).HasMaxLength(500);
        });

        base.OnModelCreating(modelBuilder);
    }
}
