using BookingSystem.Api.DTOs;

namespace BookingSystem.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync(string userId, bool isAdmin);
    Task<ReportStats> GetReportStatsAsync(string userId, bool isAdmin, DateTime? fromDate, DateTime? toDate);
}
