var builder = WebApplication.CreateBuilder(args);

// register OpenAPI/Swagger services
builder.Services.AddOpenApi();

var app = builder.Build();

// Enable OpenAPI / Swagger UI
if (app.Environment.IsDevelopment())
{
    // serves the OpenAPI/Swagger JSON: http://localhost:5199/openapi/v1.json
    app.MapOpenApi();

    // configuring swagger view: http://localhost:5199/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

var todos = new List<Todo>
{
    new()
    {
        Title = "Buy groceries – milk, bread, eggs",
        isCompleted = false
    }
};

app.MapGet("/todos", (HttpContext context) =>
{
    var todosWithId = todos
    .Select((todo, index) => new
    {
        Id = index,
        todo.Title,
        todo.isCompleted
    })
    .ToList();

    return Results.Ok(todosWithId);
})
.WithName("GetTodos");

app.MapPost("/todo", (string newTodo) =>
{
    if (string.IsNullOrWhiteSpace(newTodo))
        return Results.BadRequest("Todo item cannot be empty.");

    todos.Add(new Todo
    {
        Title = newTodo,
        isCompleted = false
    });

    return Results.Created($"/todos/{todos.Count - 1}", newTodo);
})
.WithName("AddTodo");

app.MapGet("/todos/{id:int}", (int id) =>
{
    if (id < 0 || id >= todos.Count)
        return Results.NotFound("Todo not found.");

    var todo = todos[id];

    return Results.Ok(new
    {
        Id = id,
        todo.Title,
        todo.isCompleted
    });
})
.WithName("GetTodoById");

app.MapPut("/todos/{id:int}", (int id, Todo updatedTodo) =>
{
    if (id < 0 || id >= todos.Count)
        return Results.NotFound("Todo not found.");

    if (string.IsNullOrWhiteSpace(updatedTodo.Title))
        return Results.BadRequest("Todo title cannot be empty.");

    todos[id].Title = updatedTodo.Title;
    todos[id].isCompleted = updatedTodo.isCompleted;

    return Results.Ok(todos[id]);
})
.WithName("UpdateTodo");

app.MapDelete("/todos/{id:int}", (int id) =>
{
    if (id < 0 || id >= todos.Count)
        return Results.NotFound("Todo not found.");

    todos.RemoveAt(id);

    return Results.NoContent();
})
.WithName("DeleteTodo");

app.Run();

class Todo
{
    public string Title { get; set; } = string.Empty;
    public bool isCompleted { get; set; }
}