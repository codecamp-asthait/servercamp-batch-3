using learning_testing.Models;

namespace learning_testing.DTOs;

/// <summary>
/// Data Transfer Object (DTO) for updating an existing Todo item.
/// Unlike CreateTodoRequest, this includes IsCompleted so clients can
/// modify the completion status along with other fields in one request.
/// </summary>
public class UpdateTodoRequest
{
    /// <summary>Updated title for the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Updated description text.</summary>
    public string? Description { get; set; }

    /// <summary>Updated completion status of the task.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Updated priority level.</summary>
    public Priority Priority { get; set; }

    /// <summary>Updated due date (null removes the due date).</summary>
    public DateTime? DueDate { get; set; }
}
