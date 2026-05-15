using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class CheckAvailabilityViewModel : ViewModelBase
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private DateTime _currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

    [ObservableProperty]
    private string _monthYearText = "";

    [ObservableProperty]
    private ObservableCollection<CalendarDayViewModel> _days = new();

    [ObservableProperty]
    private List<HallResponse> _halls = new();

    [ObservableProperty]
    private HallResponse? _selectedHall;

    public CheckAvailabilityViewModel()
    {
        _apiService = new ApiService();
        MonthYearText = CurrentDate.ToString("MMMM yyyy");
        LoadHallsCommand = new AsyncRelayCommand(LoadHallsAsync);
        NextMonthCommand = new RelayCommand(NextMonth);
        PreviousMonthCommand = new RelayCommand(PreviousMonth);
        TodayCommand = new RelayCommand(GoToToday);
    }

    public IAsyncRelayCommand LoadHallsCommand { get; }
    public IRelayCommand NextMonthCommand { get; }
    public IRelayCommand PreviousMonthCommand { get; }
    public IRelayCommand TodayCommand { get; }

    private async Task LoadHallsAsync()
    {
        var halls = await _apiService.GetHallsAsync();
        if (halls != null && halls.Count > 0)
        {
            Halls = halls;
            SelectedHall = Halls.FirstOrDefault();
            await RefreshCalendarAsync();
        }
    }

    partial void OnSelectedHallChanged(HallResponse? value)
    {
        _ = RefreshCalendarAsync();
    }

    private void NextMonth()
    {
        CurrentDate = CurrentDate.AddMonths(1);
        MonthYearText = CurrentDate.ToString("MMMM yyyy");
        _ = RefreshCalendarAsync();
    }

    private void PreviousMonth()
    {
        CurrentDate = CurrentDate.AddMonths(-1);
        MonthYearText = CurrentDate.ToString("MMMM yyyy");
        _ = RefreshCalendarAsync();
    }

    private void GoToToday()
    {
        CurrentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        MonthYearText = CurrentDate.ToString("MMMM yyyy");
        _ = RefreshCalendarAsync();
    }

    private async Task RefreshCalendarAsync()
    {
        if (SelectedHall == null) return;

        var startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        // Calculate the start of the calendar grid (may include days from previous month)
        var firstDayOfWeek = (int)startOfMonth.DayOfWeek;
        var startDate = startOfMonth.AddDays(-firstDayOfWeek);

        // Fetch bookings for the month
        var bookings = await _apiService.GetBookingsByHallAsync(SelectedHall.Id, startDate, startDate.AddDays(42));

        // Convert bookings to local time for correct date comparison
        if (bookings != null)
        {
            foreach (var b in bookings)
            {
                b.StartTime = b.StartTime.ToLocalTime();
                b.EndTime = b.EndTime.ToLocalTime();
            }
        }

        var newDays = new ObservableCollection<CalendarDayViewModel>();

        for (int i = 0; i < 42; i++)
        {
            var date = startDate.AddDays(i);
            var dayBookings = bookings?.Where(b => b.StartTime.Date <= date.Date && b.EndTime.Date >= date.Date).ToList() ?? new List<BookingResponse>();
            
            newDays.Add(new CalendarDayViewModel
            {
                Date = date,
                DayNumber = date.Day.ToString(),
                IsCurrentMonth = date.Month == CurrentDate.Month,
                Bookings = new ObservableCollection<BookingResponse>(dayBookings)
            });
        }

        Days = newDays;
    }
}

public partial class CalendarDayViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private string _dayNumber = "";

    [ObservableProperty]
    private bool _isCurrentMonth;

    [ObservableProperty]
    private ObservableCollection<BookingResponse> _bookings = new();

    public string StatusColor => GetStatusColor();
    public string TextColor => GetTextColor();

    private string GetStatusColor()
    {
        if (Bookings.Count == 0) return "#E6F4EA"; // Green-ish (Available)
        if (Bookings.Any(b => b.Status == 1)) return "#FCE8E8"; // Red-ish (Fully Booked/Approved)
        return "#FFF7E6"; // Orange-ish (Pending)
    }

    private string GetTextColor()
    {
        if (Bookings.Count == 0) return "#1E8E3E";
        if (Bookings.Any(b => b.Status == 1)) return "#D93025";
        return "#FA8C16";
    }

    public double Opacity => IsCurrentMonth ? 1.0 : 0.4;
}
