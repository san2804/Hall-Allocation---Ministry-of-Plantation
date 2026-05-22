using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class AlertWindow : Window
    {
        private bool _result = false;

        public AlertWindow()
        {
            InitializeComponent();
        }

        public static async Task Show(string title, string message, Window? owner = null)
        {
            var dialog = new AlertWindow();
            dialog.TitleText.Text = title;
            dialog.MessageText.Text = message;
            dialog.Title = title;

            var activeOwner = owner ?? (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (activeOwner != null) await dialog.ShowDialog(activeOwner);
            else dialog.Show();
        }

        public static async Task<bool> Confirm(string title, string message, Window? owner = null)
        {
            var dialog = new AlertWindow();
            dialog.TitleText.Text = title;
            dialog.MessageText.Text = message;
            dialog.Title = title;
            dialog.NoBtn.IsVisible = true;
            dialog.OkBtn.Content = "Yes";

            var activeOwner = owner ?? (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (activeOwner != null) await dialog.ShowDialog(activeOwner);
            else dialog.Show();

            return dialog._result;
        }

        private void Ok_Click(object? sender, RoutedEventArgs e)
        {
            _result = true;
            this.Close();
        }

        private void No_Click(object? sender, RoutedEventArgs e)
        {
            _result = false;
            this.Close();
        }
    }
}
