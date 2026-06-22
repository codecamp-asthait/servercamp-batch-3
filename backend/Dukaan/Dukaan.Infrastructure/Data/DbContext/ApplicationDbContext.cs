using System.Reflection;
using Dukaan.Domain.Entities;
using Dukaan.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Application.Models;
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
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// Gets or sets the Tenants table.
    /// </summary>
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategorizedProduct> CategorizedProducts => Set<CategorizedProduct>();
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

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

        builder.Entity<Merchant>()
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Merchant>(m => m.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Customer>()
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Customer>(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Cart>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Address>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.Addresses)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>()
            .OwnsOne(o => o.BillingAddress);

        builder.Entity<Order>()
            .OwnsOne(o => o.DeliveryAddress);

        var method = typeof(ApplicationDbContext)
            .GetMethod(nameof(SetQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType)) continue;

            if (entityType.ClrType == typeof(ApplicationUser))
            {
                builder.Entity<ApplicationUser>().HasQueryFilter(u =>
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