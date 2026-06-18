using Dukaan.Media.Host;
using Dukaan.Media.Host.Middleware;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediaServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();

app.Run();
