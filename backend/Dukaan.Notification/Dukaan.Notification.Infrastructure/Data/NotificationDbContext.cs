using Microsoft.EntityFrameworkCore;
using Dukaan.Notification.Domain.Entities;
using Dukaan.Notification.Application.Interfaces;

namespace Dukaan.Notification.Infrastructure.Data;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options, ITenantProvider tenantProvider)
    : DbContext(options)
{
    public DbSet<NotificationEntity> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notification");

        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TenantId);
            entity.HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId());
            entity.HasIndex(e => new { e.CustomerId, e.TenantId, e.IsRead, e.CreatedAt })
                  .IsDescending(false, false, false, true);
            entity.Property(e => e.EventType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ChannelType).HasMaxLength(20).IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(2000).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
