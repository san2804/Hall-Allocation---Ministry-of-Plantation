using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using BookingSystem.App.Services;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class DashboardOverviewView : UserControl
    {
        public DashboardOverviewView() : this(false) { }

        public DashboardOverviewView(bool isAdmin)
        {
            InitializeComponent();
            var viewModel = new ViewModels.DashboardOverviewViewModel(isAdmin);
            this.DataContext = viewModel;
            _ = viewModel.LoadDataCommand.ExecuteAsync(null);
        }

        private void QuickAction_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                // Better way to get the parent window in Avalonia
                var dashboardWindow = TopLevel.GetTopLevel(this) as DashboardWindow;
                if (dashboardWindow == null) return;

                switch (btn.Name)
                {
                    case "QuickAvailabilityBtn":
                        NavigateInParent(dashboardWindow, new CheckAvailabilityView(), "Check Availability", "AvailabilityBtn");
                        break;
                    case "QuickNewBookingBtn":
                        NavigateInParent(dashboardWindow, new NewBookingView(), "New Booking", "NewBookingBtn");
                        break;
                    case "QuickMyBookingsBtn":
                        NavigateInParent(dashboardWindow, new MyBookingsView(), "My Bookings", "MyBookingsBtn");
                        break;
                    case "QuickHallsBtn":
                        NavigateInParent(dashboardWindow, new HallDetailsView(), "Hall Details", "HallsBtn");
                        break;
                }
            }
        }

        private void NavigateInParent(DashboardWindow window, UserControl view, string title, string btnName)
        {
            // Try to find the button in the window's name scope
            var btn = window.FindControl<Button>(btnName);
            if (btn != null)
            {
                window.NavigateTo(view, title, btn);
            }
            else
            {
                // Fallback: If for some reason FindControl fails, still navigate but with a null button
                // (The sidebar highlight might not update, but the view will change)
                // However, NavigateTo expects a Button, so we should find a default one if needed.
                var fallbackBtn = window.FindControl<Button>("DashboardBtn");
                if (fallbackBtn != null)
                {
                    window.NavigateTo(view, title, fallbackBtn);
                }
            }
        }
    }
}
