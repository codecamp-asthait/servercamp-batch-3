using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure the database connection for PostgreSQL.
var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=codecamp_db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Endpoint to create a new user and demonstrate Entity Framework change tracking.
app.MapPost("/user", async (AppDbContext context, UserRequest req) =>
{
    // Map request to domain entity.
    var user = new User
    {
        Name = req.Name,
        Email = req.Email
    };

    // Log entity state before adding it to the context.
    var entriesBeforeAdd = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });
    
    Console.WriteLine();
    Console.WriteLine($"entriesBeforeAdd");
    foreach(var entry in entriesBeforeAdd)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }

    // Add the new user to the context. This marks it as 'Added' in the change tracker.
    await context.Users.AddAsync(user);

    // Log entity state after adding it to the context but before saving.
    var entriesAfterAdd = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });
    

    Console.WriteLine();
    Console.WriteLine($"entriesAfterAdd");
    foreach(var entry in entriesAfterAdd)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }
        
    // Persist changes to the database.
    await context.SaveChangesAsync();

    // Log entity state after saving changes. The state should now be 'Unchanged'.
    var entriesAfterSaveChange = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });

    Console.WriteLine();
    Console.WriteLine($"entriesAfterSaveChange");
    foreach(var entry in entriesAfterSaveChange)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }
});

app.Run();