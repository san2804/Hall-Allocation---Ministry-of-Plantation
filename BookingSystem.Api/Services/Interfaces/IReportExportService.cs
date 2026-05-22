using System.IO;
using System.Threading.Tasks;

namespace BookingSystem.Api.Services.Interfaces;

public interface IReportExportService
{
    Task<byte[]> ExportBookingsToExcelAsync(string userId, bool isAdmin, DateTime? fromDate, DateTime? toDate);
}
