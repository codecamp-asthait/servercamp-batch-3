using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=codecampdb";
var npsqlConnection = new NpgsqlConnection(connectionString);

var app = builder.Build();

app.MapPost("/todos", async () =>
{
    var todoService = new TodoService();
    await todoService.AddTodoAsync("First todo", npsqlConnection);
});

app.Run();