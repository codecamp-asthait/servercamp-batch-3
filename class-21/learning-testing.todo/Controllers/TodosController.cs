using learning_testing.DTOs;
using learning_testing.Models;
using learning_testing.Services;
using Microsoft.AspNetCore.Mvc;

namespace learning_testing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

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

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] CreateTodoRequest request)
    {
        var todo = await _todoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> CreateBulk([FromBody] CreateTodoRequest[] requests)
    {
        var todos = await _todoService.CreateBulkAsync(requests);
        return CreatedAtAction(nameof(GetAll), null, todos);
    }

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

    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteBulk([FromBody] Guid[] ids)
    {
        await _todoService.DeleteBulkAsync(ids);
        return NoContent();
    }
}
