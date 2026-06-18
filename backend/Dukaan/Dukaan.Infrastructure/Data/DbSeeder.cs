using Dukaan.Application.Models;
using Dukaan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Dukaan.Infrastructure.Data.DbContext;

namespace Dukaan.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Categories.AsNoTracking().IgnoreQueryFilters().AnyAsync() ||
            await context.Products.AsNoTracking().IgnoreQueryFilters().AnyAsync())
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return;
        }

        Console.WriteLine("Seeding database...");

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var tenant = await GetOrCreateTenantAsync(context);
            Console.WriteLine($"  → Tenant: {tenant.Id}");

            var user = await CreateApplicationUserAsync(userManager, tenant.Id);
            Console.WriteLine($"  → ApplicationUser: {user.Email}");

            await context.Merchants.AddAsync(new Merchant { ApplicationUserId = user.Id, TenantId = tenant.Id });
            await context.SaveChangesAsync();
            Console.WriteLine("  → Merchant profile created");

            var customerUser = await CreateCustomerAsync(userManager, context, tenant.Id);
            Console.WriteLine($"  → Customer: {customerUser.Email} (profile created)");

            var categories = CreateCategories(tenant.Id);
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"  → Categories: {categories.Count}");

            var products = CreateProducts(tenant.Id);
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"  → Products: {products.Count}");

            var associations = CreateProductCategoryAssociations(categories, products, tenant.Id);
            await context.CategorizedProducts.AddRangeAsync(associations);
            await context.SaveChangesAsync();
            Console.WriteLine($"  → Associations: {associations.Count}");

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task<Tenant> GetOrCreateTenantAsync(ApplicationDbContext context)
    {
        var tenant = await context.Tenants.AsNoTracking().FirstOrDefaultAsync();
        if (tenant != null) return tenant;

        tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            StoreName = "Demo Store",
            Slug = "demo-store",
            Category = "General"
        };
        await context.Tenants.AddAsync(tenant);
        await context.SaveChangesAsync();
        return tenant;
    }

    private static async Task<ApplicationUser> CreateApplicationUserAsync(UserManager<ApplicationUser> userManager, Guid tenantId)
    {
        var existing = await userManager.FindByEmailAsync("demo@example.com");
        if (existing != null) return existing;

        var user = new ApplicationUser
        {
            UserName = "demo@example.com",
            Email = "demo@example.com",
            EmailConfirmed = true,
            TenantId = tenantId,
            UserType = UserType.Merchant
        };

        var result = await userManager.CreateAsync(user, "Demo@123");
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return user;
    }

    private static async Task<ApplicationUser> CreateCustomerAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, Guid tenantId)
    {
        var existing = await userManager.FindByEmailAsync("customer@example.com");
        if (existing != null) return existing;

        var user = new ApplicationUser
        {
            UserName = "customer@example.com",
            Email = "customer@example.com",
            EmailConfirmed = true,
            TenantId = tenantId,
            UserType = UserType.Customer
        };

        var result = await userManager.CreateAsync(user, "Customer@123");
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await context.Customers.AddAsync(new Customer
        {
            ApplicationUserId = user.Id,
            TenantId = tenantId,
            FirstName = "Demo",
            LastName = "Customer",
            Phone = "+8801000000000"
        });
        await context.SaveChangesAsync();

        return user;
    }

    private static List<Category> CreateCategories(Guid tenantId)
    {
        var electronics = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Electronics", Description = "Electronic devices and accessories" };
        var clothing = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Clothing", Description = "Apparel and fashion items" };
        var books = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Books", Description = "Books and publications" };
        var home = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Home & Kitchen", Description = "Home and kitchen essentials" };

        var phones = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Phones", ParentCategoryId = electronics.Id };
        var laptops = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Laptops", ParentCategoryId = electronics.Id };
        var menClothing = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Men", ParentCategoryId = clothing.Id };
        var womenClothing = new Category { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Women", ParentCategoryId = clothing.Id };

        return [electronics, clothing, books, home, phones, laptops, menClothing, womenClothing];
    }

    private static List<Product> CreateProducts(Guid tenantId)
    {
        return
        [
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "iPhone 15 Pro", Description = "Latest Apple smartphone", Price = 999.99m, StockQuantity = 50 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Samsung Galaxy S24", Description = "Flagship Android phone", Price = 899.99m, StockQuantity = 45 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "MacBook Pro 16\"", Description = "Powerful laptop for professionals", Price = 2499.99m, StockQuantity = 20 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Dell XPS 15", Description = "High-performance Windows laptop", Price = 1799.99m, StockQuantity = 30 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Men's Cotton T-Shirt", Description = "Comfortable casual wear", Price = 19.99m, StockQuantity = 200 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Women's Summer Dress", Description = "Elegant summer outfit", Price = 49.99m, StockQuantity = 100 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Clean Code", Description = "Robert C. Martin's programming classic", Price = 39.99m, StockQuantity = 75 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "The Pragmatic Programmer", Description = "Essential software development guide", Price = 44.99m, StockQuantity = 60 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Coffee Maker", Description = "Automatic drip coffee maker", Price = 79.99m, StockQuantity = 40 },
            new Product { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Blender", Description = "High-speed kitchen blender", Price = 59.99m, StockQuantity = 55 }
        ];
    }

    private static List<CategorizedProduct> CreateProductCategoryAssociations(List<Category> categories, List<Product> products, Guid tenantId)
    {
        var phones = categories.First(c => c.Name == "Phones");
        var laptops = categories.First(c => c.Name == "Laptops");
        var menClothing = categories.First(c => c.Name == "Men");
        var womenClothing = categories.First(c => c.Name == "Women");
        var books = categories.First(c => c.Name == "Books");
        var home = categories.First(c => c.Name == "Home & Kitchen");

        return
        [
            new CategorizedProduct { ProductId = products[0].Id, CategoryId = phones.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[1].Id, CategoryId = phones.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[2].Id, CategoryId = laptops.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[3].Id, CategoryId = laptops.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[4].Id, CategoryId = menClothing.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[5].Id, CategoryId = womenClothing.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[6].Id, CategoryId = books.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[7].Id, CategoryId = books.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[8].Id, CategoryId = home.Id, TenantId = tenantId },
            new CategorizedProduct { ProductId = products[9].Id, CategoryId = home.Id, TenantId = tenantId }
        ];
    }
}
