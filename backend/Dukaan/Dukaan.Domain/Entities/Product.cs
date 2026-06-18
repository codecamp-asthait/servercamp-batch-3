using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents a product entity in a multi-tenant store.
/// </summary>
public class Product : ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the tenant that owns this product.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets an optional description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets an optional URL for the product's image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the pending media ID when an image upload is in progress.
    /// </summary>
    public Guid? PendingMediaId { get; set; }

    /// <summary>
    /// Gets or sets the available stock quantity.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active and visible in the store.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of category associations for this product.
    /// </summary>
    public virtual ICollection<CategorizedProduct> ProductCategories { get; set; } = [];
}