using Dukaan.Notification.Application.Interfaces;
using Dukaan.Notification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dukaan.Notification.Infrastructure.Data.Repositories;

public class Repository<T>(NotificationDbContext dbContext) : IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FindAsync(new object[] { id }, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.Where(predicate).ToListAsync(cancellationToken)
            : await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, CancellationToken cancellationToken = default) =>
        trackChanges
            ? await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken)
            : await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
