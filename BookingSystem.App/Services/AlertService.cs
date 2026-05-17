using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using BookingSystem.App.Views;

namespace BookingSystem.App.Services;

public static class AlertService
{
    public static async Task ShowAlert(string title, string message, Window? parent = null)
    {
        try
        {
            await AlertWindow.Show(title, message, parent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AlertService Error] {ex.Message}");
        }
    }

    public static async Task<bool> ShowConfirm(string title, string message, Window? parent = null)
    {
        try
        {
            return await AlertWindow.Confirm(title, message, parent);
        }
        catch
        {
            return true; 
        }
    }
}
