using learning_testing.DTOs;
using learning_testing.Models;

namespace learning_testing.Services;

/// <summary>
/// Service layer interface encapsulating business logic for Todo operations.
/// Acts as an intermediary between the API controllers and the data repository,
/// handling mapping, validation, and business rules.
/// </summary>
public interface ITodoService
{
    /// <summary>Returns todos matching the provided filter/sort criteria.</summary>
    Task<IEnumerable<TodoResponse>> GetAllAsync(TodoFilter filter);

    /// <summary>Searches todos by keyword in title or description.</summary>
    Task<IEnumerable<TodoResponse>> SearchAsync(string query);

    /// <summary>Gets a single todo by ID. Throws KeyNotFoundException if missing.</summary>
    Task<TodoResponse> GetByIdAsync(Guid id);

    /// <summary>Creates a new todo from the request DTO.</summary>
    Task<TodoResponse> CreateAsync(CreateTodoRequest request);

    /// <summary>Creates multiple todos in one operation.</summary>
    Task<IEnumerable<TodoResponse>> CreateBulkAsync(IEnumerable<CreateTodoRequest> requests);

    /// <summary>Updates an existing todo. Throws KeyNotFoundException if missing.</summary>
    Task<TodoResponse> UpdateAsync(Guid id, UpdateTodoRequest request);

    /// <summary>Toggles the IsCompleted flag of a todo.</summary>
    Task<TodoResponse> ToggleCompleteAsync(Guid id);

    /// <summary>Updates just the priority of a todo.</summary>
    Task<TodoResponse> UpdatePriorityAsync(Guid id, Priority priority);

    /// <summary>Deletes a todo by ID. Throws KeyNotFoundException if missing.</summary>
    Task DeleteAsync(Guid id);

    /// <summary>Deletes multiple todos by their IDs.</summary>
    Task DeleteBulkAsync(IEnumerable<Guid> ids);
}
