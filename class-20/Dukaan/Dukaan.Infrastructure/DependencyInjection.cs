using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Application.Models;
using Dukaan.Infrastructure.Data.Repositories;
using Dukaan.Infrastructure.Data.Services;
using Dukaan.Infrastructure.Identity.Interfaces;
using Dukaan.Infrastructure.Interceptors;
using Dukaan.Infrastructure.Services;
using Dukaan.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dukaan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<TenantInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<TenantInterceptor>());
        });

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IApplicationUserManager, IApplicationUserManager>();
        services.AddScoped<IApplicationUserManagerAdapter, IApplicationUserManagerAdapter>();

        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddHttpContextAccessor();

        return services;
    }
}
