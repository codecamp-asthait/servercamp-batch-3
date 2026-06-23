using learning_testing.Data;
using learning_testing.DTOs;
using learning_testing.Models;
using Microsoft.EntityFrameworkCore;

namespace learning_testing.Repositories;

/// <summary>
/// Entity Framework Core implementation of ITodoRepository.
/// This class handles all database operations, using LINQ queries
/// that EF Core translates into SQL for PostgreSQL.
/// </summary>
public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor injection — the DbContext is provided by the DI container
    /// with its connection string already configured (see Program.cs).
    /// </summary>
    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Builds a filtered and sorted query dynamically.
    /// Uses IQueryable<T> so filtering/sorting happens in SQL
    /// (not in memory), which is efficient for large datasets.
    /// </summary>
    public async Task<IEnumerable<Todo>> GetAllAsync(TodoFilter filter)
    {
        IQueryable<Todo> query = _context.Todos;

        if (filter.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == filter.IsCompleted.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == filter.Priority.Value);
        }

        // Switch expression maps the sortBy field name to an OrderBy call.
        query = filter.SortBy?.ToLower() switch
        {
            "updatedat" => filter.SortDir?.ToLower() == "asc" 
                ? query.OrderBy(t => t.UpdatedAt) 
                : query.OrderByDescending(t => t.UpdatedAt),
            "priority" => filter.SortDir?.ToLower() == "asc" 
                ? query.OrderBy(t => t.Priority) 
                : query.OrderByDescending(t => t.Priority),
            "duedate" => filter.SortDir?.ToLower() == "asc" 
                ? query.OrderBy(t => t.DueDate) 
                : query.OrderByDescending(t => t.DueDate),
            _ => filter.SortDir?.ToLower() == "asc" 
                ? query.OrderBy(t => t.CreatedAt) 
                : query.OrderByDescending(t => t.CreatedAt)
        };

        return await query.ToListAsync();
    }

    /// <summary>
    /// Searches todos whose title or description contains the query string.
    /// Uses LIKE in SQL via EF Core's Contains translation.
    /// </summary>
    public async Task<IEnumerable<Todo>> SearchAsync(string query)
    {
        return await _context.Todos
            .Where(t => t.Title.Contains(query) || 
                       (t.Description != null && t.Description.Contains(query)))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>Looks up a single todo by primary key.</summary>
    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }

    /// <summary>Adds a new todo to the change tracker and saves.</summary>
    public async Task<Todo> CreateAsync(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    /// <summary>Adds multiple todos in one batch (single SQL round-trip).</summary>
    public async Task<IEnumerable<Todo>> CreateBulkAsync(IEnumerable<Todo> todos)
    {
        _context.Todos.AddRange(todos);
        await _context.SaveChangesAsync();
        return todos;
    }

    /// <summary>Marks an existing todo as modified and saves changes.</summary>
    public async Task<Todo> UpdateAsync(Todo todo)
    {
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    /// <summary>Deletes a todo by ID if it exists.</summary>
    public async Task DeleteAsync(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo != null)
        {
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Deletes multiple todos matching the given IDs.</summary>
    public async Task DeleteBulkAsync(IEnumerable<Guid> ids)
    {
        var todos = await _context.Todos
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
        
        _context.Todos.RemoveRange(todos);
        await _context.SaveChangesAsync();
    }
}
