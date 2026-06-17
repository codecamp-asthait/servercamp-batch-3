using learning_testing.DTOs;
using learning_testing.Models;

namespace learning_testing.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllAsync(TodoFilter filter);
    Task<IEnumerable<Todo>> SearchAsync(string query);
    Task<Todo?> GetByIdAsync(Guid id);
    Task<Todo> CreateAsync(Todo todo);
    Task<IEnumerable<Todo>> CreateBulkAsync(IEnumerable<Todo> todos);
    Task<Todo> UpdateAsync(Todo todo);
    Task DeleteAsync(Guid id);
    Task DeleteBulkAsync(IEnumerable<Guid> ids);
}
