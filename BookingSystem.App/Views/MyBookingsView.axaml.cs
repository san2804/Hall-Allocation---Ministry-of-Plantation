using Avalonia.Controls;
using BookingSystem.App.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class MyBookingsView : UserControl
    {
        private ViewModels.MyBookingsViewModel? _viewModel;

        public MyBookingsView() : this(false) { }

        public MyBookingsView(bool isAdminMode)
        {
            InitializeComponent();
            _viewModel = new ViewModels.MyBookingsViewModel(isAdminMode);
            this.DataContext = _viewModel;
            _ = _viewModel.LoadBookingsCommand.ExecuteAsync(null);
        }

        private async void ActionBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BookingResponse booking)
            {
                if (btn.Content?.ToString() == "✅") // Approve
                {
                    await _viewModel!.UpdateStatusCommand.ExecuteAsync(new ViewModels.BookingStatusUpdateArgs
                    {
                        BookingId = booking.Id,
                        NewStatus = 1 // Approved
                    });
                }
                else if (btn.Content?.ToString() == "❌") // Reject/Cancel
                {
                    // For rejection, we could show a dialog for remarks, but for now let's just update
                    await _viewModel!.UpdateStatusCommand.ExecuteAsync(new ViewModels.BookingStatusUpdateArgs
                    {
                        BookingId = booking.Id,
                        NewStatus = _viewModel.IsAdminMode ? 2 : 3 // 2: Rejected (Admin), 3: Cancelled (User)
                    });
                }
            }
        }
    }
}
