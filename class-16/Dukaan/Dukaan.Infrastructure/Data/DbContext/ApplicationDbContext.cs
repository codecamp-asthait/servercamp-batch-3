using System.Reflection;
using Dukaan.Domain.Entities;
using Dukaan.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.Model;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dukaan.Infrastructure.Data.DbContext;

/// <summary>
/// The primary database context for the application.
/// </summary>
/// <remarks>
/// It inherits from <see cref="IdentityDbContext"/> to include ASP.NET Core Identity tables 
/// (Users, Roles, etc.) and adds the <see cref="Tenant"/> table.
/// </remarks>
public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
    : IdentityDbContext<Merchant, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// Gets or sets the Tenants table.
    /// </summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategorizedProduct> CategorizedProducts => Set<CategorizedProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CategorizedProduct>()
            .HasKey(cp => new { cp.CategoryId, cp.ProductId });

        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(SetQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType)) continue;

            if (entityType.ClrType == typeof(Merchant))
            {
                builder.Entity<Merchant>().HasQueryFilter(u =>
                    tenantProvider.GetTenantId() == null || u.TenantId == tenantProvider.GetTenantId());
            }
            else
            {
                method!.MakeGenericMethod(entityType.ClrType).Invoke(this, [builder]);
            }
        }
    }

    private void SetQueryFilter<T>(ModelBuilder builder) where T : class, ITenantEntity
    {
        builder.Entity<T>().HasQueryFilter(e => e.TenantId == tenantProvider.GetTenantId());
    }
}