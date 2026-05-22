using BookingSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IReportExportService _reportExportService;

    public DashboardController(IDashboardService dashboardService, IReportExportService reportExportService)
    {
        _dashboardService = dashboardService;
        _reportExportService = reportExportService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var stats = await _dashboardService.GetStatsAsync(userId, isAdmin);
        
        return Ok(stats);
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReportStats([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var stats = await _dashboardService.GetReportStatsAsync(userId, isAdmin, fromDate, toDate);

        return Ok(stats);
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var content = await _reportExportService.ExportBookingsToExcelAsync(userId, isAdmin, fromDate, toDate);

        var fileName = $"BookingReport_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
