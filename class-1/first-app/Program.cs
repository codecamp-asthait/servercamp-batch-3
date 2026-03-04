// step 1: configure the WebApplication
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

// step 2: build the WebApplication
var app = builder.Build();
app.MapOpenApi();

// step 3: run the WebApplication
app.Run();
