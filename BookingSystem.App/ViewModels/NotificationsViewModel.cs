using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class NotificationsViewModel : ViewModelBase
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<NotificationItemViewModel> _notifications = new();

    public NotificationsViewModel()
    {
        _apiService = new ApiService();
        LoadNotificationsCommand = new AsyncRelayCommand(LoadNotificationsAsync);
    }

    public IAsyncRelayCommand LoadNotificationsCommand { get; }

    private async Task LoadNotificationsAsync()
    {
        var bookings = await _apiService.GetMyBookingsAsync();
        if (bookings == null) return;

        var newNotifs = new ObservableCollection<NotificationItemViewModel>();

        // Sort by most recent first
        foreach (var booking in bookings.OrderByDescending(b => b.Id))
        {
            string title = "";
            string message = "";
            string icon = "";
            string color = "";

            switch (booking.Status)
            {
                case 0: // Pending
                    title = "Booking Request Submitted";
                    message = $"Your booking for {booking.HallName} on {booking.StartTime:dd MMM yyyy} is currently pending approval.";
                    icon = "⏳";
                    color = "#FA8C16"; // Orange
                    break;
                case 1: // Approved
                    title = "Booking Approved";
                    message = $"Great news! Your booking for {booking.HallName} on {booking.StartTime:dd MMM yyyy} has been approved.";
                    icon = "✅";
                    color = "#1E8E3E"; // Green
                    break;
                case 2: // Rejected
                    title = "Booking Rejected";
                    message = $"Unfortunately, your booking for {booking.HallName} on {booking.StartTime:dd MMM yyyy} was rejected.";
                    icon = "❌";
                    color = "#D93025"; // Red
                    break;
                case 3: // Cancelled
                    title = "Booking Cancelled";
                    message = $"The booking for {booking.HallName} on {booking.StartTime:dd MMM yyyy} has been cancelled.";
                    icon = "🚫";
                    color = "#666666"; // Gray
                    break;
            }

            if (!string.IsNullOrEmpty(title))
            {
                newNotifs.Add(new NotificationItemViewModel
                {
                    Title = title,
                    Message = message,
                    Icon = icon,
                    Color = color,
                    TimeAgo = "Just now" // Simplified for now
                });
            }
        }

        Notifications = newNotifs;
    }
}

public partial class NotificationItemViewModel : ObservableObject
{
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _message = "";
    [ObservableProperty] private string _icon = "";
    [ObservableProperty] private string _color = "";
    [ObservableProperty] private string _timeAgo = "";
}
