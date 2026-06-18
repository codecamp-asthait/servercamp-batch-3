namespace Dukaan.Domain.Entities;

/// <summary>
/// Represents a Tenant (Store) in the system.
/// </summary>
/// <remarks>
/// In a multi-tenant architecture, a Tenant is the top-level entity that owns its own data.
/// Each merchant belongs to exactly one tenant.
/// </remarks>
public class Tenant
{
    /// <summary>
    /// Unique identifier for the Tenant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the store.
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// A URL-friendly version of the store name.
    /// </summary>
    /// <remarks>
    /// This is typically used in the URL (e.g., Dukaan.com/my-store) to identify the tenant.
    /// </remarks>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// The business category of the store (e.g., Electronics, Fashion).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The country where the store is based.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// The currency code used for transactions in the store (e.g., USD, BDT).
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the tenant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}