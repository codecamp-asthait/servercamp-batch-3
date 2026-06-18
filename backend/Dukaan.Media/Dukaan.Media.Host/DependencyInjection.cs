using System.Text;
using Dukaan.Media.Application;
using Dukaan.Media.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Dukaan.Media.Host;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
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
            });

        return services;
    }
}
