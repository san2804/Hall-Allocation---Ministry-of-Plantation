using Avalonia.Controls;

namespace BookingSystem.App.Views
{
    public partial class NotificationsView : UserControl
    {
        public NotificationsView()
        {
            InitializeComponent();
            var viewModel = new ViewModels.NotificationsViewModel();
            this.DataContext = viewModel;
            _ = viewModel.LoadNotificationsCommand.ExecuteAsync(null);
        }
    }
}