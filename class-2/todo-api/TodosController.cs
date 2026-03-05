using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers;

[ApiController]
[Route("todos")]
public class TodosController : ControllerBase
{
    // In-memory store (same as your minimal API)
    private static readonly List<Todo> todos =
    [
        new Todo
        {
            Title = "Buy groceries – milk, bread, eggs",
            isCompleted = false
        }
    ];

    // GET /todos
    [HttpGet]
    public IActionResult GetTodos()
    {
        var todosWithId = todos.Select((todo, index) => new
        {
            Id = index,
            todo.Title,
            todo.isCompleted
        });

        return Ok(todosWithId);
    }

    // POST /todo
    [HttpPost("/todo")]
    public IActionResult AddTodo([FromBody] string newTodo)
    {
        if (string.IsNullOrWhiteSpace(newTodo))
            return BadRequest("Todo item cannot be empty.");

        todos.Add(new Todo
        {
            Title = newTodo,
            isCompleted = false
        });

        return Created($"/todos/{todos.Count - 1}", newTodo);
    }

    // GET /todos/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetTodoById(int id)
    {
        if (id < 0 || id >= todos.Count)
            return NotFound("Todo not found.");

        var todo = todos[id];

        return Ok(new
        {
            Id = id,
            todo.Title,
            todo.isCompleted
        });
    }

    // PUT /todos/{id}
    [HttpPut("{id:int}")]
    public IActionResult UpdateTodo(int id, [FromBody] Todo updatedTodo)
    {
        if (id < 0 || id >= todos.Count)
            return NotFound("Todo not found.");

        if (string.IsNullOrWhiteSpace(updatedTodo.Title))
            return BadRequest("Todo title cannot be empty.");

        todos[id].Title = updatedTodo.Title;
        todos[id].isCompleted = updatedTodo.isCompleted;

        return Ok(todos[id]);
    }

    // DELETE /todos/{id}
    [HttpDelete("{id:int}")]
    public IActionResult DeleteTodo(int id)
    {
        if (id < 0 || id >= todos.Count)
            return NotFound("Todo not found.");

        todos.RemoveAt(id);
        return NoContent();
    }
}