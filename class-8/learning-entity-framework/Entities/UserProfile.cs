public class UserProfile
{
    public Guid Id { get; set; }
    public string Address { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}