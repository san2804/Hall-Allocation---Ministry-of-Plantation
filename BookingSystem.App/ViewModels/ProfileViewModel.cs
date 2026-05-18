using System;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Initials))]
    private string _fullName = "";

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _role = "";

    [ObservableProperty]
    private string _initials = "??";

    // Password fields
    [ObservableProperty] private string _currentPassword = "";
    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _confirmPassword = "";

    public ProfileViewModel()
    {
        _apiService = new ApiService();
        LoadProfileCommand = new AsyncRelayCommand(LoadProfileAsync);
        UpdateProfileCommand = new AsyncRelayCommand(UpdateProfileAsync);
        UpdatePasswordCommand = new AsyncRelayCommand(UpdatePasswordAsync);
    }

    public IAsyncRelayCommand LoadProfileCommand { get; }
    public IAsyncRelayCommand UpdateProfileCommand { get; }
    public IAsyncRelayCommand UpdatePasswordCommand { get; }

    private async Task LoadProfileAsync()
    {
        var user = await _apiService.GetMyProfileAsync();
        if (user != null)
        {
            FullName = user.FullName;
            Username = user.Username;
            Role = user.Role;
            UpdateInitials();
        }
    }

    private void UpdateInitials()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            Initials = "??";
            return;
        }

        var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1) Initials = parts[0][0].ToString().ToUpper();
        else Initials = (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpper();
    }

    private async Task UpdateProfileAsync()
    {
        var request = new UpdateUserRequest
        {
            FullName = FullName,
            Role = Role
            // Password remains null unless changed in security section
        };

        var success = await _apiService.UpdateMyProfileAsync(request);
        if (success)
        {
            await AlertService.ShowAlert("Success", "Profile updated successfully.");
            UpdateInitials();
        }
        else
        {
            await AlertService.ShowAlert("Error", "Failed to update profile.");
        }
    }

    private async Task UpdatePasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            await AlertService.ShowAlert("Validation Error", "Please enter a new password.");
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            await AlertService.ShowAlert("Validation Error", "Passwords do not match.");
            return;
        }

        var request = new UpdateUserRequest
        {
            FullName = FullName,
            Role = Role,
            Password = NewPassword
        };

        var success = await _apiService.UpdateMyProfileAsync(request);
        if (success)
        {
            await AlertService.ShowAlert("Success", "Password updated successfully.");
            NewPassword = "";
            ConfirmPassword = "";
            CurrentPassword = "";
        }
        else
        {
            await AlertService.ShowAlert("Error", "Failed to update password.");
        }
    }
}
