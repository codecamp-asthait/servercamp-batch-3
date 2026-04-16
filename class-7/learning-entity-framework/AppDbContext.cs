using Microsoft.EntityFrameworkCore;

/// <summary>
/// Database context for the application, representing a session with the database.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets the set of users in the database.
    /// </summary>
    public DbSet<User> Users => Set<User>();
}