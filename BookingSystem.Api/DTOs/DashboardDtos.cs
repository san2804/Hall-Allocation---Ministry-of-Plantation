namespace BookingSystem.Api.DTOs;

public class DashboardStats
{
    public int TotalBookings { get; set; }
    public int UpcomingBookings { get; set; }
    public int PendingApprovals { get; set; }
    public int ThisMonthBookings { get; set; }
}

public class ReportStats
{
    public int TotalBookings { get; set; }
    public int ApprovedBookings { get; set; }
    public int PendingBookings { get; set; }
    public int RejectedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public List<PurposeStat> ByPurpose { get; set; } = new();
    public List<MonthlyOccupancyStat> MonthlyOccupancy { get; set; } = new();
}

public class PurposeStat
{
    public string Purpose { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyOccupancyStat
{
    public string Month { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
