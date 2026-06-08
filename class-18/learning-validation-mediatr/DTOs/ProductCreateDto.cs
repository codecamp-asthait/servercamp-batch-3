namespace learning_validation_mediatr.DTOs;

/// <summary>
/// DTO for creating a new product.
/// Validated by <see cref="learning_validation_mediatr.Validators.ProductCreateDtoValidator"/>
/// via FluentValidation (annotation-based validation is disabled).
/// </summary>
public class ProductCreateDto
{
    /// <summary>Name of the product (2–100 characters, required).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Price of the product. Must be greater than 0.</summary>
    public decimal Price { get; set; }
}
