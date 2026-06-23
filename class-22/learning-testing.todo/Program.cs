using Hangfire;
using Hangfire.PostgreSql;
using learning_testing.BackgroundServices;
using learning_testing.Data;
using learning_testing.Repositories;
using learning_testing.Services;
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

// ──────────────────────────────────────────────────────────────────────────
// Hangfire — Background job processing
//
// Hangfire is a fire-and-forget / recurring job framework that stores
// job state in PostgreSQL. The server (AddHangfireServer) runs in-process
// and processes enqueued jobs. The dashboard at /hangfire provides a
// web UI for monitoring and manual job management.
//
// Packages used:
//   Hangfire.AspNetCore   — ASP.NET Core integration
//   Hangfire.Core         — Core job engine
//   Hangfire.PostgreSql   — PostgreSQL storage provider
//
// Hangfire creates its own schema (hangfire.* tables) automatically
// on first run — no EF migration needed for Hangfire itself.
// ──────────────────────────────────────────────────────────────────────────
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

// Register Entity Framework Core with PostgreSQL using the connection
// string from appsettings.json (key: "DefaultConnection").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services with DI (Dependency Injection).
// AddScoped creates one instance per HTTP request.
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Alternative: use BackgroundService instead of Hangfire.
// To enable, comment out the Hangfire sections above and uncomment below.
// builder.Services.AddHostedService<OverdueTodoArchiveService>();

var app = builder.Build();

// Enable Swagger UI only in development for safety.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mount the Hangfire dashboard at /hangfire for monitoring.
// Access it at: http://localhost:xxxx/hangfire
// The dashboard shows job status, retries, failures, and a manual trigger.
app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();

// ──────────────────────────────────────────────────────────────────────────
// Recurring job registration
//
// Registers the overdue-todo archiving job to run every minute.
// Hangfire stores this schedule in PostgreSQL, so it survives restarts.
//
// Cron expressions:
//   Cron.Minutely   — every minute (for dev/testing)
//   Cron.Hourly()   — every hour
//   Cron.Daily()    — daily at midnight
//   "*/5 * * * *"  — every 5 minutes (custom)
//
// The job class (OverDueTodoArchieveJob) is resolved from DI automatically.
// ──────────────────────────────────────────────────────────────────────────
RecurringJob.AddOrUpdate<OverDueTodoArchieveJob>(
    "archive-overdue-todos",
    job => job.ArchiveOverdueTodos(),
    Cron.Minutely);

app.MapControllers();

// Start the Kestrel web server and begin accepting requests.
// Once running:
//   - API:       http://localhost:5182/api/todos
//   - Swagger:   http://localhost:5182/swagger
//   - Hangfire:  http://localhost:5182/hangfire
app.Run();
