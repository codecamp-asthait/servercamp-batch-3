using Microsoft.EntityFrameworkCore;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Application.Interfaces;
using System.Linq.Expressions;
using System.Linq;

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
    /// Applies the specified include expressions to the query.
    /// </summary>
    /// <param name="query">The base queryable.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>The queryable with includes applied.</returns>
    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
    {
        foreach (var include in includes)
        {
            var path = ExpressionPathHelper.GetIncludePath(include);
            if (path is not null) query = query.Include(path);
        }

        return query;
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FindAsync(new object[] { id }, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, "Id") == id,
                cancellationToken);

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier with optional related entities included.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes);

        return trackChanges
            ? await query.FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, "Id") == id,
                cancellationToken)
            : await query.AsNoTracking().FirstOrDefaultAsync(
                e => EF.Property<Guid>(e, "Id") == id,
                cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of all entities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple containing the items and the total count of entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var count = await _dbSet.CountAsync(cancellationToken);
        var items = trackChanges
            ? await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
            : await _dbSet
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return (items, count);
    }

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.Where(predicate).ToListAsync(cancellationToken)
            : await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate with optional related entities included via LINQ expressions.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query (supports nested collection includes).</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);

        return trackChanges
            ? await query.ToListAsync(cancellationToken)
            : await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously finds the first entity matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for this entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The first matching entity if found; otherwise, null.</returns>
    public async Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    /// <summary>
    /// Asynchronously finds the first entity matching the specified predicate with optional related entities included.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for this entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>The first matching entity if found; otherwise, null.</returns>
    public async Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);

        return trackChanges
            ? await query.FirstOrDefaultAsync(cancellationToken)
            : await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(predicate);
        var count = await query.CountAsync(cancellationToken);
        var items = trackChanges
            ? await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
            : await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return (items, count);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate, with optional related entities included.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);
        var count = await query.CountAsync(cancellationToken);

        var items = trackChanges
            ? await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
            : await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return (items, count);
    }

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate,
    /// ordered by the specified expression, with optional related entities included.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="orderBy">An expression to order the results.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_dbSet, includes).Where(predicate);
        var orderedQuery = orderBy(query);
        var count = await query.CountAsync(cancellationToken);

        var items = trackChanges
            ? await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
            : await orderedQuery
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return (items, count);
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

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
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
}
