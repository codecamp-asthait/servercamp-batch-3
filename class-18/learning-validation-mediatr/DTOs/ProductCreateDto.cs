using System.ComponentModel.DataAnnotations;

namespace learning_validation_mediatr.DTOs;

/// <summary>DTO for creating a new product. Validated via data annotations.</summary>
public class ProductCreateDto
{
    /// <summary>Name of the product (2–100 characters, required).</summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Price of the product. Must be greater than 0.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }
}
