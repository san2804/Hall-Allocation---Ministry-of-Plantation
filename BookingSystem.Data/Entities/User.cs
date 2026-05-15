namespace BookingSystem.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Officer"; // "Admin" or "Officer"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
