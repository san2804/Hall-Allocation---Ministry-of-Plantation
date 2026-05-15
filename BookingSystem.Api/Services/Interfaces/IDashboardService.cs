using BookingSystem.Api.DTOs;

namespace BookingSystem.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync(string userId, bool isAdmin);
}
