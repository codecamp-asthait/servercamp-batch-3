using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

// Apply pending migrations automatically on startup
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<TodoDb>().Database.Migrate();

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToListAsync());

app.MapGet("/todos/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id) is Todo todo ? Results.Ok(todo) : Results.NotFound());

app.MapPost("/todos", async (TodoRequest req, TodoDb db) =>
{
    var todo = new Todo { Title = req.Title };
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (int id, TodoRequest req, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.Title = req.Title;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPatch("/todos/{id}/complete", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    todo.IsCompleted = true;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
