using Avalonia.Controls;
using BookingSystem.App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class MyBookingsView : UserControl
    {
        private readonly ApiService _apiService;

        public MyBookingsView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ = LoadBookings();
        }

        private async Task LoadBookings()
        {
            var bookings = await _apiService.GetMyBookingsAsync();
            if (bookings != null)
            {
                foreach (var b in bookings)
                {
                    b.StartTime = b.StartTime.ToLocalTime();
                    b.EndTime = b.EndTime.ToLocalTime();
                }

                var itemsControl = this.FindControl<ItemsControl>("BookingsItemsControl");
                if (itemsControl != null)
                {
                    itemsControl.ItemsSource = bookings;
                }

                var showingText = this.FindControl<TextBlock>("ShowingText");
                if (showingText != null)
                {
                    showingText.Text = $"Showing {bookings.Count} entries";
                }
            }
        }
    }
}
