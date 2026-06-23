using learning_testing.Data;
using learning_testing.Services;
using learning_testing.Repositories;
using Microsoft.EntityFrameworkCore;

// ──────────────────────────────────────────────────────────────────────────
// Application entry point.
//
// This file uses the "top-level statements" feature (C# 10+) which
// eliminates the explicit class / Main method boilerplate. The compiler
// generates a Program class with a Main entry point automatically.
// ──────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers so the framework can route HTTP requests.
builder.Services.AddControllers();

// Register Swagger / OpenAPI for interactive API documentation (dev only).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Entity Framework Core with PostgreSQL using the connection
// string from appsettings.json (key: "DefaultConnection").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services with DI (Dependency Injection).
// AddScoped creates one instance per HTTP request.
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Enable Swagger UI only in development for safety.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Start the Kestrel web server and begin accepting requests.
app.Run();
