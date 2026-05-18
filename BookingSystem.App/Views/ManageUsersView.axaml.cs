using Avalonia.Controls;

namespace BookingSystem.App.Views
{
    public partial class ManageUsersView : UserControl
    {
        public ManageUsersView()
        {
            InitializeComponent();
            var viewModel = new ViewModels.ManageUsersViewModel();
            this.DataContext = viewModel;
            _ = viewModel.LoadUsersCommand.ExecuteAsync(null);
        }
    }
}