using FluentValidation;
using FluentValidation.AspNetCore;
using learning_validation_mediatr.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register FluentValidation and disable the built-in annotation-based validation
// so only FluentValidation rules are applied.
builder.Services.AddFluentValidationAutoValidation(config =>
    config.DisableDataAnnotationsValidation = true);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.MapControllers();

app.Run();
