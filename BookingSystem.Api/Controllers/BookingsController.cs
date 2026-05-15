using BookingSystem.Api.DTOs;
using BookingSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var bookings = await _bookingService.GetByUserAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet("hall/{hallId}")]
    public async Task<IActionResult> GetByHall(int hallId, [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var bookings = await _bookingService.GetByHallAsync(hallId, start, end);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Check for conflicts
        if (await _bookingService.HasConflictAsync(request.HallId, request.StartTime, request.EndTime))
        {
            return BadRequest(new { Message = "This hall is already booked for the selected time slot." });
        }

        var result = await _bookingService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetMyBookings), new { id = result.Id }, result);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusRequest request)
    {
        var success = await _bookingService.UpdateStatusAsync(id, request.Status, request.AdminRemark);
        if (!success) return NotFound();
        return NoContent();
    }
}
