using learning_validation_mediatr.DTOs;

namespace learning_validation_mediatr.Services;

/// <summary>Defines operations for managing products.</summary>
public interface IProductService
{
    /// <summary>Returns all products.</summary>
    IEnumerable<Product> GetAll();

    /// <summary>Returns a product by ID, or <c>null</c> if not found.</summary>
    /// <param name="id">The product ID.</param>
    Product? GetById(int id);

    /// <summary>Creates and stores a new product from the given DTO.</summary>
    /// <param name="dto">Validated product creation data.</param>
    /// <returns>The newly created product with its assigned ID.</returns>
    Product Create(ProductCreateDto dto);
}
