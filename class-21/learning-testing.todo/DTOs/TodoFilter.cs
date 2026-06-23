using learning_testing.Models;

namespace learning_testing.DTOs;

/// <summary>
/// Filtering and sorting parameters passed as query string to the GET /api/todos endpoint.
/// Demonstrates how to build dynamic query logic in the repository layer
/// based on optional client-provided criteria.
/// </summary>
public class TodoFilter
{
    /// <summary>If set, filters todos by completion status (true = completed, false = pending).</summary>
    public bool? IsCompleted { get; set; }

    /// <summary>If set, filters todos by priority level.</summary>
    public Priority? Priority { get; set; }

    /// <summary>Field to sort by: "createdAt" (default), "updatedAt", "priority", or "dueDate".</summary>
    public string SortBy { get; set; } = "createdAt";

    /// <summary>Sort direction: "desc" (default) or "asc".</summary>
    public string SortDir { get; set; } = "desc";
}
