using Avalonia.Controls;
using BookingSystem.App.Services;
using System.Threading.Tasks;

namespace BookingSystem.App.Views
{
    public partial class HallDetailsView : UserControl
    {
        private readonly ApiService _apiService;

        public HallDetailsView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ = LoadHalls();
        }

        private async Task LoadHalls()
        {
            var halls = await _apiService.GetHallsAsync();
            if (halls != null)
            {
                var itemsControl = this.FindControl<ItemsControl>("HallsItemsControl");
                if (itemsControl != null)
                {
                    itemsControl.ItemsSource = halls;
                }
            }
        }
    }
}
