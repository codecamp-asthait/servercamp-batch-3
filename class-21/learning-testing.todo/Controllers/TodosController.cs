using learning_testing.DTOs;
using learning_testing.Models;
using learning_testing.Services;
using Microsoft.AspNetCore.Mvc;

namespace learning_testing.Controllers;

/// <summary>
/// REST API controller for managing Todo items.
/// 
/// Follows the standard CRUD pattern plus extra endpoints for bulk operations,
/// searching, toggling completion, and updating priority.
/// All endpoints return DTOs (TodoResponse) rather than domain entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    /// <summary>Constructor injection: receives the service layer via DI.</summary>
    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    /// <summary>GET /api/todos — Returns all todos with optional filtering and sorting.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAll(
        [FromQuery] bool? isCompleted,
        [FromQuery] Priority? priority,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir)
    {
        var filter = new TodoFilter
        {
            IsCompleted = isCompleted,
            Priority = priority,
            SortBy = sortBy ?? "createdAt",
            SortDir = sortDir ?? "desc"
        };

        var todos = await _todoService.GetAllAsync(filter);
        return Ok(todos);
    }

    /// <summary>GET /api/todos/search?q=keyword — Searches todos by title or description.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query cannot be empty");
        }

        var todos = await _todoService.SearchAsync(q);
        return Ok(todos);
    }

    /// <summary>GET /api/todos/{id} — Returns a single todo by ID.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoResponse>> GetById(Guid id)
    {
        try
        {
            var todo = await _todoService.GetByIdAsync(id);
            return Ok(todo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>POST /api/todos — Creates a new todo. Returns 201 with the created resource.</summary>
    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] CreateTodoRequest request)
    {
        var todo = await _todoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    /// <summary>POST /api/todos/bulk — Creates multiple todos at once. Returns 201.</summary>
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> CreateBulk([FromBody] CreateTodoRequest[] requests)
    {
        var todos = await _todoService.CreateBulkAsync(requests);
        return CreatedAtAction(nameof(GetAll), null, todos);
    }

    /// <summary>PUT /api/todos/{id} — Fully updates an existing todo.</summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TodoResponse>> Update(Guid id, [FromBody] UpdateTodoRequest request)
    {
        try
        {
            var todo = await _todoService.UpdateAsync(id, request);
            return Ok(todo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>PATCH /api/todos/{id}/complete — Toggles the IsCompleted flag.</summary>
    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoResponse>> ToggleComplete(Guid id)
    {
        try
        {
            var todo = await _todoService.ToggleCompleteAsync(id);
            return Ok(todo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>PATCH /api/todos/{id}/priority — Updates just the priority field.</summary>
    [HttpPatch("{id}/priority")]
    public async Task<ActionResult<TodoResponse>> UpdatePriority(Guid id, [FromBody] Priority priority)
    {
        try
        {
            var todo = await _todoService.UpdatePriorityAsync(id, priority);
            return Ok(todo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>DELETE /api/todos/{id} — Deletes a todo by ID. Returns 204 on success.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _todoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>DELETE /api/todos/bulk — Deletes multiple todos by their IDs. Returns 204.</summary>
    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteBulk([FromBody] Guid[] ids)
    {
        await _todoService.DeleteBulkAsync(ids);
        return NoContent();
    }
}
