using BookingSystem.Api.DTOs;
using BookingSystem.Api.Services.Interfaces;
using BookingSystem.Data;
using BookingSystem.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetStatsAsync(string userId, bool isAdmin)
    {
        if (!int.TryParse(userId, out int id)) return new DashboardStats();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var query = _context.Bookings.AsQueryable();
        
        if (!isAdmin)
        {
            query = query.Where(b => b.UserId == id);
        }

        return new DashboardStats
        {
            TotalBookings = await query.CountAsync(),
            UpcomingBookings = await query.CountAsync(b => b.StartTime > now && b.Status == BookingStatus.Approved),
            PendingApprovals = await query.CountAsync(b => b.Status == BookingStatus.Pending),
            ThisMonthBookings = await query.CountAsync(b => b.CreatedAt >= startOfMonth)
        };
    }

    public async Task<ReportStats> GetReportStatsAsync(string userId, bool isAdmin, DateTime? fromDate, DateTime? toDate)
    {
        if (!int.TryParse(userId, out int id)) return new ReportStats();

        var query = _context.Bookings.AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(b => b.UserId == id);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(b => b.StartTime >= fromDate.Value.ToUniversalTime());
        }
        if (toDate.HasValue)
        {
            query = query.Where(b => b.StartTime <= toDate.Value.ToUniversalTime());
        }

        var bookings = await query.ToListAsync();
        var total = bookings.Count;

        var stats = new ReportStats
        {
            TotalBookings = total,
            ApprovedBookings = bookings.Count(b => b.Status == BookingStatus.Approved),
            PendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending),
            RejectedBookings = bookings.Count(b => b.Status == BookingStatus.Rejected),
            CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
        };

        if (total > 0)
        {
            stats.ByPurpose = bookings.GroupBy(b => b.Purpose)
                .Select(g => new PurposeStat
                {
                    Purpose = g.Key,
                    Count = g.Count(),
                    Percentage = Math.Round((double)g.Count() / total * 100, 1)
                })
                .OrderByDescending(p => p.Count)
                .ToList();
        }

        // Monthly occupancy for the last 6 months
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
        sixMonthsAgo = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var monthlyQuery = _context.Bookings.Where(b => b.StartTime >= sixMonthsAgo && b.Status == BookingStatus.Approved);
        if (!isAdmin)
        {
            monthlyQuery = monthlyQuery.Where(b => b.UserId == id);
        }

        var monthlyData = await monthlyQuery.ToListAsync();

        for (int i = 0; i < 6; i++)
        {
            var targetMonth = sixMonthsAgo.AddMonths(i);
            var monthBookings = monthlyData.Count(b => b.StartTime.Year == targetMonth.Year && b.StartTime.Month == targetMonth.Month);
            
            // Simple logic: assume 30 slots per month for percentage calculation (just for visual representation)
            stats.MonthlyOccupancy.Add(new MonthlyOccupancyStat
            {
                Month = targetMonth.ToString("MMM"),
                Percentage = Math.Min(100, Math.Round((double)monthBookings / 30 * 100, 1))
            });
        }

        return stats;
    }
}
