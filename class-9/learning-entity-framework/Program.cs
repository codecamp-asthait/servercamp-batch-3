using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=postgres;Database=codecamp_db";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.AddInterceptors(
        new SaveChangesTimingInterceptor(),
        new QueryLoggingInterceptor()
    );
});

var app = builder.Build();

app.MapPost("/user", async (AppDbContext context, UserRequest req) =>
{
    var user = new User
    {
        Name = req.Name,
        Email = req.Email
    };

    var entriesBeforeAdd = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });

    Console.WriteLine();
    Console.WriteLine($"entriesBeforeAdd");
    foreach (var entry in entriesBeforeAdd)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }


    await context.Users.AddAsync(user);

    var entriesAfterAdd = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });


    Console.WriteLine();
    Console.WriteLine($"entriesAfterAdd");
    foreach (var entry in entriesAfterAdd)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }


    await context.SaveChangesAsync();

    var entriesAfterSaveChange = context.ChangeTracker.Entries()
        .Select(e => new
        {
            EntityName = e.Entity.GetType().Name,
            State = e.State.ToString()
        });

    Console.WriteLine();
    Console.WriteLine($"entriesAfterSaveChange");
    foreach (var entry in entriesAfterSaveChange)
    {
        Console.WriteLine($"Entity: {entry.EntityName}");
        Console.WriteLine($"State: {entry.State}");
        Console.WriteLine("--------------------------------------------");
    }
});

app.MapPost("/one-to-one", async (AppDbContext context, UserRequestWithAddress req) =>
{
    var userEntity = new User
    {
        Name = req.Name,
        Email = req.Email,
        UserProfile = new UserProfile
        {
            Address = req.Address
        }
    };

    await context.Users.AddAsync(userEntity);
    await context.SaveChangesAsync();
});

app.MapPost("/one-to-many", async (AppDbContext context, UserRequestWithOrder req) =>
{
    var user = new User
    {
        Name = req.Name,
        Email = req.Email,
        UserProfile = new UserProfile
        {
            Address = req.Address
        },
        UserOrders = []
    };

    foreach (var item in req.Orders)
    {
        user.UserOrders.Add(new Order
        {
            Total = item.Total
        });
    }

    await context.Users.AddAsync(user);
    await context.SaveChangesAsync();
});

app.MapPost("/many-to-many", async (AppDbContext context) =>
{
    var student = new Student
    {
        Name = "Student 1"
    };

    var course = new Course
    {
        Title = "C# Basics"
    };

    var enrollment = new Enrollment
    {
        Student = student,
        Course = course
    };

    await context.Enrollments.AddAsync(enrollment);
    await context.SaveChangesAsync();
});

app.MapGet("/students-courses", async (AppDbContext context) =>
{
    var students = context.Students
        .Include(e => e.Enrollments)
        .ThenInclude(e => e.Course)
        .Select(e => new 
        {
            e.Name,
            Course = e.Enrollments.Select(c => c.Course.Title).ToList()     
        });

    return Results.Ok(students);

    // var student = await context.Students.FirstOrDefaultAsync();
    // return Results.Ok(student);
});


app.Run();