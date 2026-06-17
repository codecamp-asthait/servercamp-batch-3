using learning_testing.Data;
using learning_testing.DTOs;
using learning_testing.Models;
using Microsoft.EntityFrameworkCore;

namespace learning_testing.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

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

    public async Task<IEnumerable<Todo>> SearchAsync(string query)
    {
        return await _context.Todos
            .Where(t => t.Title.Contains(query) || 
                       (t.Description != null && t.Description.Contains(query)))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task<IEnumerable<Todo>> CreateBulkAsync(IEnumerable<Todo> todos)
    {
        _context.Todos.AddRange(todos);
        await _context.SaveChangesAsync();
        return todos;
    }

    public async Task<Todo> UpdateAsync(Todo todo)
    {
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
        return todo;
    }

    public async Task DeleteAsync(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo != null)
        {
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBulkAsync(IEnumerable<Guid> ids)
    {
        var todos = await _context.Todos
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
        
        _context.Todos.RemoveRange(todos);
        await _context.SaveChangesAsync();
    }
}
