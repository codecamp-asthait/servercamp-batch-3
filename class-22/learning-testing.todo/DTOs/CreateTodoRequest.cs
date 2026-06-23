using learning_testing.Models;

namespace learning_testing.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for creating a new Todo item.
/// DTOs separate the internal domain model from the external API contract,
/// allowing the API to evolve independently of the database schema.
/// </summary>
public class CreateTodoRequest
{
    /// <summary>Required title of the new todo task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description providing more details about the task.</summary>
    public string? Description { get; set; }

    /// <summary>Priority level for the new task (defaults to Low).</summary>
    public Priority Priority { get; set; }

    /// <summary>Optional due date for task completion.</summary>
    public DateTime? DueDate { get; set; }
}
