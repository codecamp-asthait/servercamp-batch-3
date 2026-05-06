using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Dukaan.Domain.Entities;

namespace Dukaan.Infrastructure.Data.DbContext;

/// <summary>
/// The primary database context for the application.
/// </summary>
/// <remarks>
/// It inherits from <see cref="IdentityDbContext"/> to include ASP.NET Core Identity tables 
/// (Users, Roles, etc.) and adds the <see cref="Tenant"/> table.
/// </remarks>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<Merchant, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// Gets or sets the Tenants table.
    /// </summary>
    public DbSet<Tenant> Tenants { get; set; }
}