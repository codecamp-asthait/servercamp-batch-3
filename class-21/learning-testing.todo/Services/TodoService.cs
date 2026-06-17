using learning_testing.DTOs;
using learning_testing.Models;
using learning_testing.Repositories;

namespace learning_testing.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponse>> GetAllAsync(TodoFilter filter)
    {
        var todos = await _repository.GetAllAsync(filter);
        return todos.Select(MapToResponse);
    }

    public async Task<IEnumerable<TodoResponse>> SearchAsync(string query)
    {
        var todos = await _repository.SearchAsync(query);
        return todos.Select(MapToResponse);
    }

    public async Task<TodoResponse> GetByIdAsync(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }
        return MapToResponse(todo);
    }

    public async Task<TodoResponse> CreateAsync(CreateTodoRequest request)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(todo);
        return MapToResponse(created);
    }

    public async Task<IEnumerable<TodoResponse>> CreateBulkAsync(IEnumerable<CreateTodoRequest> requests)
    {
        var todos = requests.Select(r => new Todo
        {
            Id = Guid.NewGuid(),
            Title = r.Title,
            Description = r.Description,
            Priority = r.Priority,
            DueDate = r.DueDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var created = await _repository.CreateBulkAsync(todos);
        return created.Select(MapToResponse);
    }

    public async Task<TodoResponse> UpdateAsync(Guid id, UpdateTodoRequest request)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;
        todo.Priority = request.Priority;
        todo.DueDate = request.DueDate;
        todo.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(todo);
        return MapToResponse(updated);
    }

    public async Task<TodoResponse> ToggleCompleteAsync(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }

        todo.IsCompleted = !todo.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(todo);
        return MapToResponse(updated);
    }

    public async Task<TodoResponse> UpdatePriorityAsync(Guid id, Priority priority)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }

        todo.Priority = priority;
        todo.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(todo);
        return MapToResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }

        await _repository.DeleteAsync(id);
    }

    public async Task DeleteBulkAsync(IEnumerable<Guid> ids)
    {
        await _repository.DeleteBulkAsync(ids);
    }

    private static TodoResponse MapToResponse(Todo todo)
    {
        return new TodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            Priority = todo.Priority.ToString(),
            DueDate = todo.DueDate,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }
}
