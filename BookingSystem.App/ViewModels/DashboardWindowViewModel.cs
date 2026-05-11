using CommunityToolkit.Mvvm.ComponentModel;

namespace BookingSystem.App.ViewModels
{
    public partial class DashboardWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNormalUser))]
        private bool _isAdmin;

        public bool IsNormalUser => !IsAdmin;

        public DashboardWindowViewModel(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }
    }
}