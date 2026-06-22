using Dukaan.Domain.Enums;
using Dukaan.Domain.Interfaces;
using Dukaan.Domain.ValueObjects;

namespace Dukaan.Domain.Entities;

public class Order : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public long SequenceNumber { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public AddressSnapshot BillingAddress { get; set; }
    public AddressSnapshot DeliveryAddress { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; } = [];
}
