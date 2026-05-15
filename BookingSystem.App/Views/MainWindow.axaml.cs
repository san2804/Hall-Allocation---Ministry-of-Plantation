using Avalonia.Controls;
using Avalonia.Interactivity;
using BookingSystem.App.Services;

namespace BookingSystem.App.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text?.Trim() ?? "";
            string password = PasswordTextBox.Text ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessageTextBlock.Text = "Please enter both username and password.";
                return;
            }

            ErrorMessageTextBlock.Text = "Logging in...";
            var result = await _apiService.LoginAsync(username, password);

            if (result == null || !result.Success)
            {
                ErrorMessageTextBlock.Text = result?.Message ?? "Invalid username or password.";
                return;
            }

            var dashboard = new DashboardWindow(result.IsAdmin);
            dashboard.Show();
            this.Close();
        }
    }
}