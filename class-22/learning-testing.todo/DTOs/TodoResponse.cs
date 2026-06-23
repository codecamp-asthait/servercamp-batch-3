using learning_testing.Models;

namespace learning_testing.DTOs;

/// <summary>
/// Data Transfer Object (DTO) returned to API consumers when
/// reading Todo data. Note that Priority is a string here
/// (not the enum) so the API returns human-readable values like "High".
/// </summary>
public class TodoResponse
{
    /// <summary>Unique identifier of the todo item.</summary>
    public Guid Id { get; set; }

    /// <summary>Title of the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Detailed description (may be null).</summary>
    public string? Description { get; set; }

    /// <summary>Whether the task is completed.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Priority as a human-readable string (e.g., "Low", "Medium", "High").</summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>Optional due date for the task.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Timestamp when the task was created (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp when the task was last updated (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}
