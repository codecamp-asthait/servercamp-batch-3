using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents the many-to-many association between a product and a category.
/// </summary>
public class CategorizedProduct : ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the category.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant this association belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the associated product.
    /// </summary>
    public virtual Product Product { get; set; }

    /// <summary>
    /// Gets or sets the associated category.
    /// </summary>
    public virtual Category Category { get; set; }
}