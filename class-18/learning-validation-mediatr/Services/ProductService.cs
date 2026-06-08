using learning_validation_mediatr.DTOs;

namespace learning_validation_mediatr.Services;

/// <summary>
/// In-memory implementation of <see cref="IProductService"/>.
/// Uses <see cref="ProductStore"/> as the backing data store.
/// </summary>
public class ProductService : IProductService
{
    /// <inheritdoc/>
    public IEnumerable<Product> GetAll() => ProductStore.Products;

    /// <inheritdoc/>
    public Product? GetById(int id) => ProductStore.Products.FirstOrDefault(p => p.Id == id);

    /// <inheritdoc/>
    public Product Create(ProductCreateDto dto)
    {
        var product = new Product
        {
            Id = ProductStore.Products.Count + 1,
            Name = dto.Name,
            Price = dto.Price
        };
        ProductStore.Products.Add(product);
        return product;
    }
}
