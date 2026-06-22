using Dukaan.Domain.Enums;
using Dukaan.Domain.Interfaces;

namespace Dukaan.Domain.Entities;

public class Address : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public string Label { get; set; } = string.Empty;
    public AddressType Type { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public virtual Customer Customer { get; set; }
}
