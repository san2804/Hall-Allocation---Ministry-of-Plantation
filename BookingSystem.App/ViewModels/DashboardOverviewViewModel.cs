using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class DashboardOverviewViewModel : ViewModelBase
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private int _totalBookings;

    [ObservableProperty]
    private int _upcomingBookingsCount;

    [ObservableProperty]
    private int _pendingApprovals;

    [ObservableProperty]
    private int _thisMonthBookings;

    [ObservableProperty]
    private ObservableCollection<BookingResponse> _upcomingBookings = new();

    public DashboardOverviewViewModel()
    {
        _apiService = new ApiService();
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    public IAsyncRelayCommand LoadDataCommand { get; }

    private async Task LoadDataAsync()
    {
        var stats = await _apiService.GetDashboardStatsAsync();
        if (stats != null)
        {
            TotalBookings = stats.TotalBookings;
            UpcomingBookingsCount = stats.UpcomingBookings;
            PendingApprovals = stats.PendingApprovals;
            ThisMonthBookings = stats.ThisMonthBookings;
        }

        var bookings = await _apiService.GetMyBookingsAsync();
        if (bookings != null)
        {
            UpcomingBookings.Clear();
            var now = System.DateTime.Now;
            foreach (var b in bookings)
            {
                // Ensure times are local for both comparison and display
                b.StartTime = b.StartTime.ToLocalTime();
                b.EndTime = b.EndTime.ToLocalTime();

                // Show both Pending (0) and Approved (1) bookings
                if (b.StartTime > now && (b.Status == 0 || b.Status == 1))
                {
                    UpcomingBookings.Add(b);
                }
            }
        }
    }
}
