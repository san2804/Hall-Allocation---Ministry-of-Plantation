using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BookingSystem.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text ?? "";
            string password = PasswordTextBox.Text ?? "";
            bool isAdmin = false;

            if (username == "admin" && password == "admin1234")
            {
                isAdmin = true;
            }
            else if (username == "normal_user" && password == "normal1234")
            {
                isAdmin = false;
            }
            else
            {
                ErrorMessageTextBlock.Text = "Invalid username or password.";
                return;
            }

            var dashboard = new DashboardWindow(isAdmin);
            dashboard.Show();
            this.Close();
        }
    }
}