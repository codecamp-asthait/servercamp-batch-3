// create the builder
var builder = WebApplicationFactory.CreateBuilder();
builder.Services.AddControllers();

// build the app
var app = builder.Build();
app.MapControllers();

app.MapGet("/codecamp", (ctx) => $"We are codecamp batch 3. Response is generated from route: {ctx.Path}");

// run the server
await app.RunAsync(5005);