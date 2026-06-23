using learning_testing.DTOs;
using learning_testing.Models;
using learning_testing.Repositories;

namespace learning_testing.Services;

/// <summary>
/// Implements the business logic for Todo operations.
/// Maps between request/response DTOs and the domain model (Todo entity),
/// enforces existence checks, and delegates persistence to the repository.
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    /// <summary>
    /// Constructor injection — receives the repository from DI.
    /// Because the dependency is an interface (ITodoRepository),
    /// the service is decoupled from the actual data access implementation.
    /// </summary>
    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Delegates to repository with filtering/sorting, maps results to response DTOs.</summary>
    public async Task<IEnumerable<TodoResponse>> GetAllAsync(TodoFilter filter)
    {
        var todos = await _repository.GetAllAsync(filter);
        return todos.Select(MapToResponse);
    }

    /// <summary>Searches todos and maps results to response DTOs.</summary>
    public async Task<IEnumerable<TodoResponse>> SearchAsync(string query)
    {
        var todos = await _repository.SearchAsync(query);
        return todos.Select(MapToResponse);
    }

    /// <summary>Fetches a todo by ID or throws if not found.</summary>
    public async Task<TodoResponse> GetByIdAsync(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }
        return MapToResponse(todo);
    }

    /// <summary>
    /// Maps the CreateTodoRequest (DTO) into a Todo domain entity,
    /// sets system-managed fields (Id, timestamps), and persists it.
    /// </summary>
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

    /// <summary>Creates multiple todos in a single operation.</summary>
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

    /// <summary>
    /// Updates an existing todo. Loads the entity first (to verify it exists),
    /// applies changes from the request DTO, and saves.
    /// </summary>
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

    /// <summary>Flips the IsCompleted flag (true → false, false → true).</summary>
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

    /// <summary>Updates only the priority of a todo.</summary>
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

    /// <summary>Deletes a todo, throwing if it doesn't exist.</summary>
    public async Task DeleteAsync(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            throw new KeyNotFoundException($"Todo with id {id} not found");
        }

        await _repository.DeleteAsync(id);
    }

    /// <summary>Deletes multiple todos — no existence check needed per item.</summary>
    public async Task DeleteBulkAsync(IEnumerable<Guid> ids)
    {
        await _repository.DeleteBulkAsync(ids);
    }

    /// <summary>
    /// Private helper that maps a Todo domain entity to a TodoResponse DTO.
    /// Note that Priority is converted to a string so the API returns "High" not 2.
    /// </summary>
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
