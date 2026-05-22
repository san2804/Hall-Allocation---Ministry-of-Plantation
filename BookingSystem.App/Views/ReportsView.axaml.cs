using Avalonia.Controls;
using BookingSystem.App.ViewModels;

namespace BookingSystem.App.Views
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
            DataContext = new ReportsViewModel();
        }
    }
}