/// <summary>Represents a product in the store.</summary>
public class Product
{
    /// <summary>Unique identifier for the product.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the product.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Price of the product in USD.</summary>
    public decimal Price { get; set; }
}

/// <summary>In-memory data store holding the product list.</summary>
public static class ProductStore
{
    /// <summary>The shared list of all products.</summary>
    public static List<Product> Products { get; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse",  Price = 29.99m  },
    ];
}
