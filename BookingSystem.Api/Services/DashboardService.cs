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
}
