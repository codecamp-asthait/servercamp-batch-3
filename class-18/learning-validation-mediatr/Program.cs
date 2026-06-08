using FluentValidation;
using learning_validation_mediatr.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register validators (no auto model-binding validation — handled by MediatR pipeline)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// MediatR: scan handlers and register ValidationBehavior as a pipeline step
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

var app = builder.Build();

app.MapControllers();

app.Run();
