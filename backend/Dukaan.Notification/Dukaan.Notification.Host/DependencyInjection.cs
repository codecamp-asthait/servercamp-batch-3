using System.Text;
using Dukaan.Notification.Application;
using Dukaan.Notification.Infrastructure;
using Dukaan.Notification.Infrastructure.Hubs;
using Dukaan.Notification.Host.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Dukaan.Notification.Host;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSignalR()
            .AddStackExchangeRedis(configuration["Redis:ConnectionString"]!, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalR");
            });

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }

    public static WebApplication UsePresentationPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseMiddleware<TenantResolutionMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<NotificationHub>("/hubs/notifications");

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}
