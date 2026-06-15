using Dukaan.Application;
using Dukaan.Infrastructure;
using Dukaan.Host;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    var observability = context.Configuration.GetSection("Observability");
    var otlpEndpoint = observability["OtlpEndpoint"]!;
    var serviceName = observability["ServiceName"]!;
    var environment = observability["Environment"]!;

    configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.OpenTelemetry(opts =>
        {
            opts.Endpoint = otlpEndpoint;
            opts.Protocol = OtlpProtocol.Grpc;
            opts.ResourceAttributes = new Dictionary<string, object>
            {
                ["service.name"] = serviceName,
                ["deployment.environment"] = environment
            };
        });
});

builder.Services
    .AddConfigurations(builder.Configuration)
    .AddObservability(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();
app.UsePresentationPipeline();

app.Run();