using Dukaan.Application.Interfaces;
using Dukaan.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dukaan.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
