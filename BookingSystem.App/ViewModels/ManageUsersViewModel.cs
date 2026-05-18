using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookingSystem.App.ViewModels;

public partial class ManageUsersViewModel : ViewModelBase
{
    private readonly ApiService _apiService;
    private List<UserDto> _allUsers = new();

    [ObservableProperty]
    private ObservableCollection<UserDto> _users = new();

    [ObservableProperty]
    private string _searchText = "";

    [ObservableProperty]
    private bool _isLoading;

    // Add User Form Fields
    [ObservableProperty] private string _newUsername = "";
    [ObservableProperty] private string _newFullName = "";
    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newRole = "Officer";

    public ObservableCollection<string> Roles { get; } = new() { "Officer", "Admin" };

    public ManageUsersViewModel()
    {
        _apiService = new ApiService();
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);
        AddUserCommand = new AsyncRelayCommand(AddUserAsync);
        DeleteUserCommand = new AsyncRelayCommand<int>(DeleteUserAsync);
    }

    public IAsyncRelayCommand LoadUsersCommand { get; }
    public IAsyncRelayCommand AddUserCommand { get; }
    public IAsyncRelayCommand<int> DeleteUserCommand { get; }

    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _apiService.GetUsersAsync();
            if (result != null)
            {
                _allUsers = result.OrderBy(u => u.FullName).ToList();
                ApplyFilter();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var filtered = _allUsers.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(u => 
                u.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        Users = new ObservableCollection<UserDto>(filtered);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private async Task AddUserAsync()
    {
        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(NewFullName))
        {
            await AlertService.ShowAlert("Validation Error", "Full Name, Username and Password are required.");
            return;
        }

        var request = new CreateUserRequest
        {
            Username = NewUsername,
            FullName = NewFullName,
            Password = NewPassword,
            Role = NewRole
        };

        var result = await _apiService.CreateUserAsync(request);
        if (result.Success)
        {
            await AlertService.ShowAlert("Success", $"User '{NewUsername}' created successfully.");
            // Reset fields
            NewUsername = "";
            NewFullName = "";
            NewPassword = "";
            await LoadUsersAsync();
        }
        else
        {
            await AlertService.ShowAlert("Error", result.Message);
        }
    }

    private async Task DeleteUserAsync(int id)
    {
        var confirm = await AlertService.ShowConfirm("Confirm Delete", "Are you sure you want to delete this user?");
        if (!confirm) return;

        var success = await _apiService.DeleteUserAsync(id);
        if (success)
        {
            await LoadUsersAsync();
        }
        else
        {
            await AlertService.ShowAlert("Error", "Failed to delete user.");
        }
    }
}
