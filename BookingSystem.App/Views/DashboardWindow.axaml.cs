using Avalonia.Controls;
using Avalonia.Interactivity;
using BookingSystem.App.Views;
using System;
using System.Linq;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class DashboardWindow : Window
    {
        private bool _isAdmin;
        public DashboardWindow() : this(false, "User", "Role") { }

        public DashboardWindow(bool isAdmin, string fullName, string role)
        {
            _isAdmin = isAdmin;
            InitializeComponent();
            this.DataContext = new ViewModels.DashboardWindowViewModel(isAdmin, fullName, role);
            // Set initial view
            NavigateTo(new DashboardOverviewView(isAdmin), "Dashboard", DashboardBtn);
        }

        private async void MenuBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Name != null)
            {
                switch (btn.Name)
                {
                    case "LogoutBtn":
                        await HandleLogout();
                        break;
                    default:
                        HandleNavigation(btn.Name, btn);
                        break;
                }
            }
        }

        private void HeaderBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "HeaderNotificationsBtn":
                        HandleNavigation("NotificationsBtn", NotificationsBtn);
                        break;
                    case "HeaderProfileBtn":
                        HandleNavigation("ProfileBtn", ProfileBtn);
                        break;
                }
            }
        }

        private void HandleNavigation(string name, Button btn)
        {
            switch (name)
            {
                case "DashboardBtn":
                    NavigateTo(new DashboardOverviewView(_isAdmin), "Dashboard", btn);
                    break;
                case "AvailabilityBtn":
                    NavigateTo(new CheckAvailabilityView(), "Check Availability", btn);
                    break;
                case "NewBookingBtn":
                    NavigateTo(new NewBookingView(), "New Booking", btn);
                    break;
                case "MyBookingsBtn":
                case "ManageBookingsBtn":
                    NavigateTo(new MyBookingsView(name == "ManageBookingsBtn"), name == "ManageBookingsBtn" ? "Manage Bookings" : "My Bookings", btn);
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
                    NavigateTo(new ManageUsersView(), "Manage Users", btn);
                    break;
                case "SystemSettingsBtn":
                    NavigateTo(new SystemSettingsView(), "System Settings", btn);
                    break;
            }
        }

        private async Task HandleLogout()
        {
            var confirm = await Services.AlertService.ShowConfirm("Logout", "Are you sure you want to logout?", this);
            if (confirm)
            {
                PerformLogout();
            }
        }

        private void PerformLogout()
        {
            new Services.ApiService().Logout();
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
        public void NavigateTo(UserControl view, string title, Button activeBtn)
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
