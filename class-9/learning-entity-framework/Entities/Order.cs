public class Order
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}