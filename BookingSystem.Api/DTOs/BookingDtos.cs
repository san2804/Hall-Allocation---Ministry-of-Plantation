using BookingSystem.Data.Enums;

namespace BookingSystem.Api.DTOs;

public class BookingRequest
{
    public int HallId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
}

public class BookingResponse
{
    public int Id { get; set; }
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public BookingStatus Status { get; set; }
    public string? AdminRemark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateBookingStatusRequest
{
    public BookingStatus Status { get; set; }
    public string? AdminRemark { get; set; }
}
