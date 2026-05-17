using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class MyBookingsViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private readonly bool _isAdminMode;
    private List<BookingResponse> _allBookings = new();

    [ObservableProperty]
    private ObservableCollection<BookingResponse> _bookings = new();

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private string _showingCountText = "Showing 0 entries";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int? _selectedStatusFilter = null; // null for "All"

    public bool IsAdminMode => _isAdminMode;

    public string PageTitle => _isAdminMode ? "Manage Bookings" : "My Bookings";

    public MyBookingsViewModel(bool isAdminMode)
    {
        _apiService = new ApiService();
        _isAdminMode = isAdminMode;
        LoadBookingsCommand = new AsyncRelayCommand(LoadBookingsAsync);
        UpdateStatusCommand = new AsyncRelayCommand<BookingStatusUpdateArgs>(UpdateStatusAsync);
        SetFilterCommand = new RelayCommand<string>(SetFilter);
    }

    public IAsyncRelayCommand LoadBookingsCommand { get; }
    public IAsyncRelayCommand<BookingStatusUpdateArgs> UpdateStatusCommand { get; }
    public IRelayCommand<string> SetFilterCommand { get; }

    private void SetFilter(string? statusStr)
    {
        if (int.TryParse(statusStr, out int status))
        {
            SelectedStatusFilter = status;
        }
        else
        {
            SelectedStatusFilter = null;
        }
        ApplyFilters();
    }

    private async Task LoadBookingsAsync()
    {
        IsLoading = true;
        try
        {
            var result = _isAdminMode 
                ? await _apiService.GetAllBookingsAsync() 
                : await _apiService.GetMyBookingsAsync();

            if (result != null)
            {
                // Convert to local time
                foreach (var b in result)
                {
                    b.StartTime = b.StartTime.ToLocalTime();
                    b.EndTime = b.EndTime.ToLocalTime();
                }

                _allBookings = result.OrderByDescending(b => b.Id).ToList();
                ApplyFilters();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilters()
    {
        var filtered = _allBookings.AsEnumerable();

        if (SelectedStatusFilter.HasValue)
        {
            filtered = filtered.Where(b => b.Status == SelectedStatusFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(b => 
                b.HallName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                b.Purpose.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                b.UserFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        Bookings = new ObservableCollection<BookingResponse>(filtered);
        UpdateShowingCount();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    private async Task UpdateStatusAsync(BookingStatusUpdateArgs? args)
    {
        if (args == null) return;

        var success = await _apiService.UpdateBookingStatusAsync(args.BookingId, args.NewStatus, args.Remark);
        if (success)
        {
            await LoadBookingsAsync();
        }
    }

    private void UpdateShowingCount()
    {
        ShowingCountText = $"Showing {Bookings.Count} entries";
    }
}

public class BookingStatusUpdateArgs
{
    public int BookingId { get; set; }
    public int NewStatus { get; set; }
    public string? Remark { get; set; }
}
