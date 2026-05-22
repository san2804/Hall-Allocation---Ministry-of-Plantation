using BookingSystem.Api.Services.Interfaces;
using BookingSystem.Data;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BookingSystem.Api.Services;

public class ReportExportService : IReportExportService
{
    private readonly AppDbContext _context;

    public ReportExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportBookingsToExcelAsync(string userId, bool isAdmin, DateTime? fromDate, DateTime? toDate)
    {
        if (!int.TryParse(userId, out int id)) return Array.Empty<byte>();

        var query = _context.Bookings
            .Include(b => b.Hall)
            .Include(b => b.User)
            .AsQueryable();

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

        var bookings = await query.OrderByDescending(b => b.StartTime).ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Bookings Report");

        // Headers
        worksheet.Cell(1, 1).Value = "Booking ID";
        worksheet.Cell(1, 2).Value = "Hall Name";
        worksheet.Cell(1, 3).Value = "Requested By";
        worksheet.Cell(1, 4).Value = "Start Time";
        worksheet.Cell(1, 5).Value = "End Time";
        worksheet.Cell(1, 6).Value = "Purpose";
        worksheet.Cell(1, 7).Value = "Status";
        worksheet.Cell(1, 8).Value = "Created At";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1a5d38");
        headerRow.Style.Font.FontColor = XLColor.White;

        // Data
        for (int i = 0; i < bookings.Count; i++)
        {
            var b = bookings[i];
            int row = i + 2;
            worksheet.Cell(row, 1).Value = $"BK{b.Id:D4}";
            worksheet.Cell(row, 2).Value = b.Hall.Name;
            worksheet.Cell(row, 3).Value = b.User.FullName;
            worksheet.Cell(row, 4).Value = b.StartTime.ToLocalTime().ToString("g");
            worksheet.Cell(row, 5).Value = b.EndTime.ToLocalTime().ToString("g");
            worksheet.Cell(row, 6).Value = b.Purpose;
            worksheet.Cell(row, 7).Value = b.Status.ToString();
            worksheet.Cell(row, 8).Value = b.CreatedAt.ToLocalTime().ToString("g");
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
