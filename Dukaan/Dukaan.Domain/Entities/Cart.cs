using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents a shopping cart belonging to a specific customer in a tenant's store.
/// </summary>
public class Cart : ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the cart.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant this cart belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the customer who owns this cart.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with this cart.
    /// </summary>
    public virtual Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the cart was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the cart was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of items in the cart.
    /// </summary>
    public virtual ICollection<CartItem> Items { get; set; } = [];
}
