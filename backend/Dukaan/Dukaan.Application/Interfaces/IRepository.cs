using System.Linq.Expressions;

namespace Dukaan.Application.Interfaces;

/// <summary>
/// A generic repository interface for basic CRUD and paged retrieval operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier with optional related entities included.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="trackChanges">Whether to track changes for this entity in the EF context.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Asynchronously retrieves a paged list of all entities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple containing the items and the total count of entities.</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of matching entities.</returns>
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate with optional related entities included via a LINQ transformer.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>A collection of matching entities.</returns>
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Asynchronously finds the first entity matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for this entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The first matching entity if found; otherwise, null.</returns>
    Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously finds the first entity matching the specified predicate with optional related entities included.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="trackChanges">Whether to track changes for this entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <returns>The first matching entity if found; otherwise, null.</returns>
    Task<T?> FindFirstAsync(
        Expression<Func<T, bool>> predicate,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Asynchronously retrieves a paged list of entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="trackChanges">Whether to track changes for these entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A tuple containing the items and the total count of matching entities.</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

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
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] includes);

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
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>> predicate,
        int pageNumber,
        int pageSize,
        bool trackChanges,
        CancellationToken cancellationToken,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    /// Persists all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
