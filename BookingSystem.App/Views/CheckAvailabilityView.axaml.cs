using Avalonia.Controls;

namespace BookingSystem.App.Views
{
    public partial class CheckAvailabilityView : UserControl
    {
        public CheckAvailabilityView()
        {
            InitializeComponent();
            var viewModel = new ViewModels.CheckAvailabilityViewModel();
            this.DataContext = viewModel;
            
            // Load initial data
            _ = viewModel.LoadHallsCommand.ExecuteAsync(null);
        }
    }
}