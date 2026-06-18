using System.Linq.Expressions;

namespace Dukaan.Media.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
