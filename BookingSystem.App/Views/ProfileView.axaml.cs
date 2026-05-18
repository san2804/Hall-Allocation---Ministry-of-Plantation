using Avalonia.Controls;

namespace BookingSystem.App.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
            var viewModel = new ViewModels.ProfileViewModel();
            this.DataContext = viewModel;
            _ = viewModel.LoadProfileCommand.ExecuteAsync(null);
        }
    }
}