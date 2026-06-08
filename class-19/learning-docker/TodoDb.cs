using Microsoft.EntityFrameworkCore;

class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

record TodoRequest(string Title);

class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}
