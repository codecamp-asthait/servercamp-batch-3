using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents a product category in the system.
/// </summary>
public class Category : ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant this category belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets an optional description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the unique identifier of the parent category, if any.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the parent category.
    /// </summary>
    public virtual Category? ParentCategory { get; set; }

    /// <summary>
    /// Gets or sets the collection of sub-categories.
    /// </summary>
    public virtual ICollection<Category> SubCategories { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of product associations for this category.
    /// </summary>
    public virtual ICollection<CategorizedProduct> ProductLinks { get; set; } = [];
}