using learning_testing.DTOs;
using learning_testing.Models;

namespace learning_testing.Repositories;

/// <summary>
/// Repository interface defining data access operations for Todo entities.
/// Following the Repository pattern abstracts away the underlying data source
/// (EF Core / PostgreSQL) from the service layer, making the code
/// testable and the data access strategy swappable.
/// </summary>
public interface ITodoRepository
{
    /// <summary>Retrieves all todos with optional filtering and sorting.</summary>
    Task<IEnumerable<Todo>> GetAllAsync(TodoFilter filter);

    /// <summary>Searches todos by keyword in title or description.</summary>
    Task<IEnumerable<Todo>> SearchAsync(string query);

    /// <summary>Finds a todo by its ID. Returns null if not found.</summary>
    Task<Todo?> GetByIdAsync(Guid id);

    /// <summary>Creates a new todo and persists it to the database.</summary>
    Task<Todo> CreateAsync(Todo todo);

    /// <summary>Creates multiple todos in a single database round-trip.</summary>
    Task<IEnumerable<Todo>> CreateBulkAsync(IEnumerable<Todo> todos);

    /// <summary>Updates an existing todo and saves changes.</summary>
    Task<Todo> UpdateAsync(Todo todo);

    /// <summary>Deletes a todo by its ID.</summary>
    Task DeleteAsync(Guid id);

    /// <summary>Deletes multiple todos by their IDs in one operation.</summary>
    Task DeleteBulkAsync(IEnumerable<Guid> ids);
}
