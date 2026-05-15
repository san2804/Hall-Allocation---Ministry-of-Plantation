using BookingSystem.Data.Enums;

namespace BookingSystem.Data.Entities;

public class Booking
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public Hall Hall { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? AdminRemark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
