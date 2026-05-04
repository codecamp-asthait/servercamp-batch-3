using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.DbContext;

namespace Dukaan.Infrastructure.Data.Repositories;

/// <summary>
/// A generic repository implementation for basic CRUD operations.
/// </summary>
/// <typeparam name="T">The type of the entity this repository manages.</typeparam>
/// <remarks>
/// The Repository pattern is used to decouple the business logic from the data access layer (EF Core).
/// This makes the code more testable and easier to maintain.
/// </remarks>
public class Repository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    /// <summary>
    /// Finds an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The Guid of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Adds a new entity to the database context.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    /// <summary>
    /// Persists all changes made in this context to the database.
    /// </summary>
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}