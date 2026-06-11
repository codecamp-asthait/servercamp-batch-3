using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents an item within a shopping cart.
/// </summary>
public class CartItem : ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the cart item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant this cart item belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the cart this item belongs to.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Gets or sets the cart associated with this item.
    /// </summary>
    public virtual Cart Cart { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product associated with this item.
    /// </summary>
    public virtual Product Product { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in the cart.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price of the product at the time it was added to the cart (snapshot).
    /// </summary>
    public decimal UnitPrice { get; set; }
}
