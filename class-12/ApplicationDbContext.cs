using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// The ApplicationDbContext represents the database session and provides APIs to interact with the data.
// By inheriting from IdentityDbContext<IdentityUser>, we automatically include all the tables 
// required for ASP.NET Core Identity (Users, Roles, Claims, etc.).
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    // You can override OnModelCreating to customize the database schema (e.g., renaming tables).
    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);

    //     builder.Entity<IdentityRole>(b =>
    //     {
    //         b.ToTable("AspNetRolesShafayet");
    //     });
    // }
}