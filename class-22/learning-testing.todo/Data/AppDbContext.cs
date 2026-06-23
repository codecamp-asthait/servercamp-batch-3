using learning_testing.Models;
using Microsoft.EntityFrameworkCore;

namespace learning_testing.Data;

/// <summary>
/// Entity Framework Core database context for the Todo application.
/// Acts as the bridge between the application and PostgreSQL database.
/// Manages entity queries, change tracking, and database schema configuration.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>Collection of Todo entities mapped to the "Todos" table.</summary>
    public DbSet<Todo> Todos { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Configures the entity mappings and constraints when the model is created.
    /// Sets primary keys, field lengths, and value conversion rules.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Priority).HasConversion<string>();
        });
    }
}
