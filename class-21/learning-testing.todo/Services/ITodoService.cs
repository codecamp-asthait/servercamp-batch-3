using learning_testing.DTOs;
using learning_testing.Models;

namespace learning_testing.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponse>> GetAllAsync(TodoFilter filter);
    Task<IEnumerable<TodoResponse>> SearchAsync(string query);
    Task<TodoResponse> GetByIdAsync(Guid id);
    Task<TodoResponse> CreateAsync(CreateTodoRequest request);
    Task<IEnumerable<TodoResponse>> CreateBulkAsync(IEnumerable<CreateTodoRequest> requests);
    Task<TodoResponse> UpdateAsync(Guid id, UpdateTodoRequest request);
    Task<TodoResponse> ToggleCompleteAsync(Guid id);
    Task<TodoResponse> UpdatePriorityAsync(Guid id, Priority priority);
    Task DeleteAsync(Guid id);
    Task DeleteBulkAsync(IEnumerable<Guid> ids);
}
