using Dukaan.Application;
using Dukaan.Infrastructure;
using Dukaan.Host;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseSerilog((_, configuration) => configuration
        .WriteTo.Console()
        .MinimumLevel.Information());
}

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();
app.UsePresentationPipeline();

app.Run();