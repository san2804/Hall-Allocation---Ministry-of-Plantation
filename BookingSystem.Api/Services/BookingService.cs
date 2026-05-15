using BookingSystem.Api.DTOs;
using BookingSystem.Api.Services.Interfaces;
using BookingSystem.Data;
using BookingSystem.Data.Entities;
using BookingSystem.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingResponse>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.Hall)
            .Include(b => b.User)
            .Select(b => MapToResponse(b))
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingResponse>> GetByUserAsync(string userId)
    {
        if (!int.TryParse(userId, out int id)) return new List<BookingResponse>();

        return await _context.Bookings
            .Include(b => b.Hall)
            .Include(b => b.User)
            .Where(b => b.UserId == id)
            .Select(b => MapToResponse(b))
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingResponse>> GetByHallAsync(int hallId, DateTime start, DateTime end)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        return await _context.Bookings
            .Include(b => b.Hall)
            .Include(b => b.User)
            .Where(b => b.HallId == hallId && 
                       b.Status != BookingStatus.Cancelled && 
                       b.Status != BookingStatus.Rejected &&
                       ((b.StartTime >= startUtc && b.StartTime <= endUtc) || 
                        (b.EndTime >= startUtc && b.EndTime <= endUtc) ||
                        (b.StartTime <= startUtc && b.EndTime >= endUtc)))
            .Select(b => MapToResponse(b))
            .ToListAsync();
    }

    public async Task<BookingResponse?> GetByIdAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Hall)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
        
        return booking != null ? MapToResponse(booking) : null;
    }

    public async Task<BookingResponse> CreateAsync(string userId, BookingRequest request)
    {
        if (!int.TryParse(userId, out int id)) throw new ArgumentException("Invalid user ID");

        var booking = new Booking
        {
            HallId = request.HallId,
            UserId = id,
            StartTime = EnsureUtc(request.StartTime),
            EndTime = EnsureUtc(request.EndTime),
            Purpose = request.Purpose,
            AdditionalInfo = request.AdditionalInfo,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Load relations for response
        await _context.Entry(booking).Reference(b => b.Hall).LoadAsync();
        await _context.Entry(booking).Reference(b => b.User).LoadAsync();

        return MapToResponse(booking);
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) return dateTime;
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
    }

    public async Task<bool> UpdateStatusAsync(int id, BookingStatus status, string? remark)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        booking.Status = status;
        booking.AdminRemark = remark;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasConflictAsync(int hallId, DateTime start, DateTime end, int? excludeBookingId = null)
    {
        var startUtc = EnsureUtc(start);
        var endUtc = EnsureUtc(end);

        return await _context.Bookings.AnyAsync(b => 
            b.HallId == hallId && 
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Rejected &&
            (!excludeBookingId.HasValue || b.Id != excludeBookingId.Value) &&
            ((startUtc >= b.StartTime && startUtc < b.EndTime) || 
             (endUtc > b.StartTime && endUtc <= b.EndTime) ||
             (startUtc <= b.StartTime && endUtc >= b.EndTime)));
    }

    private static BookingResponse MapToResponse(Booking b) => new BookingResponse
    {
        Id = b.Id,
        HallId = b.HallId,
        HallName = b.Hall?.Name ?? "Unknown",
        UserId = b.UserId.ToString(),
        UserFullName = b.User?.FullName ?? "Unknown",
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        Purpose = b.Purpose,
        AdditionalInfo = b.AdditionalInfo,
        Status = b.Status,
        AdminRemark = b.AdminRemark,
        CreatedAt = b.CreatedAt
    };
}
