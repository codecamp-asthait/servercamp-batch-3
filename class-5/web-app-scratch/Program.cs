// create the builder
var builder = WebApplicationFactory.CreateBuilder();
builder.Services.AddControllers();

// register dependencies
builder.Services.AddTransient<TestService>();
builder.Services.AddTransient<TestChildService>();

// build the app
var app = builder.Build();
app.MapControllers();

// resolving dependencies
var testService = app.Services.GetService<TestService>();
testService.Print();

app.MapGet("/codecamp", (ctx) => $"We are codecamp batch 3. Response is generated from route: {ctx.Path}");

// run the server
await app.RunAsync(5005);