using Avalonia.Controls;
using Avalonia.Interactivity;
using BookingSystem.App.Views;
using System.Linq;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow() : this(false) { }

        public DashboardWindow(bool isAdmin)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.DashboardWindowViewModel(isAdmin);
            // Set initial view
            NavigateTo(new DashboardOverviewView(), "Dashboard", DashboardBtn);
        }

        private async void MenuBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Content?.ToString() == "Logout")
                {
                    await HandleLogout();
                    return;
                }

                switch (btn.Name)
                {
                    case "DashboardBtn":
                        NavigateTo(new DashboardOverviewView(), "Dashboard", btn);
                        break;
                    case "AvailabilityBtn":
                        NavigateTo(new CheckAvailabilityView(), "Check Availability", btn);
                        break;
                    case "NewBookingBtn":
                        NavigateTo(new NewBookingView(), "New Booking", btn);
                        break;
                    case "MyBookingsBtn":
                    case "ManageBookingsBtn":
                        NavigateTo(new MyBookingsView(), btn.Name == "ManageBookingsBtn" ? "Manage Bookings" : "My Bookings", btn);
                        break;
                    case "HallsBtn":
                        NavigateTo(new HallDetailsView(), "Hall Details", btn);
                        break;
                    case "ProfileBtn":
                        NavigateTo(new ProfileView(), "Profile", btn);
                        break;
                    case "NotificationsBtn":
                        NavigateTo(new NotificationsView(), "Notifications", btn);
                        break;
                    case "ReportsBtn":
                        NavigateTo(new ReportsView(), "Reports", btn);
                        break;
                    case "HelpBtn":
                        NavigateTo(new HelpView(), "Help & Support", btn);
                        break;
                    case "ManageUsersBtn":
                    case "SystemSettingsBtn":
                        // Placeholders for Admin specific views not yet implemented
                        break;
                }
            }
        }

        private async Task HandleLogout()
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Logout", "Are you sure you want to logout?", MessageBox.Avalonia.Enums.ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question);
            var result = await box.ShowAsPopupAsync(this);

            if (result == MsBox.Avalonia.Enums.ButtonResult.Yes)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void NavigateTo(UserControl view, string title, Button activeBtn)
        {
            MainContent.Content = view;
            PageTitle.Text = title;

            // Update sidebar button active state
            var sidebar = activeBtn.Parent as StackPanel;
            if (sidebar != null)
            {
                foreach (var child in sidebar.Children.OfType<Button>())
                {
                    child.Classes.Remove("Active");
                }
                activeBtn.Classes.Add("Active");
            }
        }
    }
}