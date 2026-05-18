namespace Dukaan.Application.Interfaces;

public interface IRepository<T>
{
    public Task<T?> GetByIdAsync(Guid id);
    public Task AddAsync(T entity);
    public Task SaveChangesAsync();
    public Task BeginTransactionAsync();
    public Task CommitTransactionAsync();
    public Task RollbackTransactionAsync();
}
