using BookingSystem.Api.DTOs;
using BookingSystem.Data.Enums;

namespace BookingSystem.Api.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingResponse>> GetAllAsync();
    Task<IEnumerable<BookingResponse>> GetByUserAsync(string userId);
    Task<IEnumerable<BookingResponse>> GetByHallAsync(int hallId, DateTime start, DateTime end);
    Task<BookingResponse?> GetByIdAsync(int id);
    Task<BookingResponse> CreateAsync(string userId, BookingRequest request);
    Task<bool> UpdateStatusAsync(int id, BookingStatus status, string? remark, string userId, bool isAdmin);
    Task<bool> HasConflictAsync(int hallId, DateTime start, DateTime end, int? excludeBookingId = null);
}
