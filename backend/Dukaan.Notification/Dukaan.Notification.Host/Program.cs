using Dukaan.Notification.Host;
using Dukaan.Notification.Application;
using Dukaan.Notification.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();
app.UsePresentationPipeline();

app.Run();
