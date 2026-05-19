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
    }
}
