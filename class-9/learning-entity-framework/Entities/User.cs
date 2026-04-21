// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// [Table("AppUsers")]
public class User
{
    public Guid Id { get; set; }

    // [Required]
    // [MaxLength(100)]
    public string Name { get; set; }

    // [Column("USER_EMAIL")]
    public string Email { get; set; }

    public UserProfile UserProfile { get; set; }
    public List<Order> UserOrders { get; set; }
}