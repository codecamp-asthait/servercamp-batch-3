using Dukaan.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Dukaan.Infrastructure.Interceptors;
using Dukaan.Infrastructure.Data.Services;
using Dukaan.Infrastructure.Data.DbContext;
using Dukaan.Infrastructure.Identity.Adapters;
using Dukaan.Infrastructure.Identity.Services;
using Dukaan.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Dukaan.Infrastructure.Identity.Interfaces;
using Dukaan.Infrastructure.Services.Interfaces;

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
        services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
        services.AddScoped<IApplicationUserManagerAdapter, ApplicationUserManagerAdapter>();

        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddHttpContextAccessor();

        return services;
    }
}
