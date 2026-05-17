using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookingSystem.App.ViewModels
{
    public partial class DashboardWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNormalUser))]
        private bool _isAdmin;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Initials))]
        private string _fullName = "";

        [ObservableProperty]
        private string _role = "";

        public bool IsNormalUser => !IsAdmin;

        public string Initials 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) return "??";
                var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
                return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpper();
            }
        }

        public DashboardWindowViewModel(bool isAdmin, string fullName, string role)
        {
            IsAdmin = isAdmin;
            FullName = fullName;
            Role = role;
        }
    }
}