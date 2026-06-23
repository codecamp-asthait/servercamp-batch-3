namespace learning_testing.Models;

/// <summary>
/// Represents a Todo task item in the system.
/// This is the core domain entity that maps to the "Todos" database table.
/// </summary>
public class Todo
{
    /// <summary>Unique identifier for the todo item.</summary>
    public Guid Id { get; set; }

    /// <summary>Short title describing the task. Required, max 200 characters.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional detailed description of the task. Max 1000 characters.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the task has been completed. Defaults to false.</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Priority level of the task (Low, Medium, High).</summary>
    public Priority Priority { get; set; }

    /// <summary>Optional due date for the task.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Timestamp when the todo was created (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp when the todo was last updated (UTC).</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Whether this todo has been archived (soft-deleted).
    /// Archived todos are excluded from the default listing
    /// and are hidden from the main view. Defaults to false.
    /// The background archiving job sets this to true for overdue items.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Timestamp (UTC) when this todo was archived.
    /// Null if the todo has never been archived.
    /// Set automatically by the archiving job alongside IsArchived.
    /// </summary>
    public DateTime? ArchivedAt { get; set; }
}

/// <summary>
/// Defines the priority levels a Todo task can have.
/// Stored as a string in the database via the configured value converter.
/// </summary>
public enum Priority
{
    /// <summary>Low priority — non-urgent tasks.</summary>
    Low = 0,

    /// <summary>Medium priority — normal importance.</summary>
    Medium = 1,

    /// <summary>High priority — urgent or important tasks.</summary>
    High = 2
}
