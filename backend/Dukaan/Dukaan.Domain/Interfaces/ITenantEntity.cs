namespace Dukaan.Domain.Interfaces;

/// <summary>
/// Interface that identifies entities that belong to a specific tenant.
/// </summary>
/// <remarks>
/// This interface is used to implement automated multi-tenancy filters.
/// Any entity implementing this interface must have a <see cref="TenantId"/>.
/// </remarks>
public interface ITenantEntity
{
    /// <summary>
    /// The unique identifier of the tenant that owns this entity.
    /// </summary>
    Guid TenantId { get; set; }
}