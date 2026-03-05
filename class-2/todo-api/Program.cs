var builder = WebApplication.CreateBuilder(args);

// register OpenAPI/Swagger services
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Enable OpenAPI / Swagger UI
if (app.Environment.IsDevelopment())
{
    // serves the OpenAPI/Swagger JSON: http://localhost:5199/openapi/v1.json
    app.MapOpenApi();

    // configuring swagger view: http://localhost:5199/swagger/index.html
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.MapControllers();

app.Run();