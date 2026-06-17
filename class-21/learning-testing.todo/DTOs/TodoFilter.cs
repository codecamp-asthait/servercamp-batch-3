using learning_testing.Models;

namespace learning_testing.DTOs;

public class TodoFilter
{
    public bool? IsCompleted { get; set; }
    public Priority? Priority { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortDir { get; set; } = "desc";
}
