using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Application.Interfaces;
using System.Linq.Expressions;

namespace Dukaan.Infrastructure.Data.Repositories;

/// <summary>
/// A generic repository implementation for basic CRUD and paged retrieval operations.
/// </summary>
/// <typeparam name="T">The type of the entity this repository manages.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false) =>
        trackChanges
            ? await _dbSet.FindAsync(id)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier with optional related entities included.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync(
        Guid id,
        bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            var path = ExpressionPathHelper.GetIncludePath(include);
            if (path != null) query = query.Include(path);
        }

        return trackChanges
            ? await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id)
            : await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of all entities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <returns>A tuple containing the items and the total count of entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool trackChanges = false)
    {
        var count = await _dbSet.CountAsync();
        var items = trackChanges
            ? await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
            : await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, count);
    }

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false) =>
        trackChanges ? await _dbSet.Where(predicate).ToListAsync() : await _dbSet.Where(predicate).AsNoTracking().ToListAsync();

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate with optional related entities included via LINQ expressions.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="includes">The related entities to include in the query (supports nested collection includes).</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            var path = ExpressionPathHelper.GetIncludePath(include);
            if (path != null) query = query.Include(path);
        }

        query = query.Where(predicate);

        return trackChanges
            ? await query.ToListAsync()
            : await query.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges = false)
    {
        var query = _dbSet.Where(predicate);
        var count = await query.CountAsync();
        var items = trackChanges
            ? await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
            : await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, count);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate, with optional related entities included.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes)
        {
            var path = ExpressionPathHelper.GetIncludePath(include);
            if (path != null) query = query.Include(path);
        }

        query = query.Where(predicate);
        var count = await query.CountAsync();

        var items = trackChanges
            ? await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
            : await query.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, count);
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    /// <summary>
    /// Marks an existing entity as modified.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity) => _dbSet.Update(entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(T entity) => _dbSet.Remove(entity);

    /// <summary>
    /// Persists all changes made in this context to the database.
    /// </summary>
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    public async Task BeginTransactionAsync() => await _dbContext.Database.BeginTransactionAsync();

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    public async Task CommitTransactionAsync() => await _dbContext.Database.CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    public async Task RollbackTransactionAsync() => await _dbContext.Database.RollbackTransactionAsync();
}
