namespace BookingSystem.Api.DTOs;

public class DashboardStats
{
    public int TotalBookings { get; set; }
    public int UpcomingBookings { get; set; }
    public int PendingApprovals { get; set; }
    public int ThisMonthBookings { get; set; }
}
