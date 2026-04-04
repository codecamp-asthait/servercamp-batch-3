var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// inline middleware
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 1 = Before next()");
    await next();
    Console.WriteLine("Middleware 1 = After next()");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 2 = Before next()");
    await next();
    Console.WriteLine("Middleware 2 = After next()");
});

app.MapGet("/", () =>
{
    Console.WriteLine("Hello World");
    return "Hello World";
});

app.MapGet("/error", () =>
{
    throw new InvalidOperationException("This is a simulated exception");
});

app.Run();


