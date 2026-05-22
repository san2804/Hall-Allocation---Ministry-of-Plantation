using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using BookingSystem.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace BookingSystem.App.ViewModels;

public partial class ReportsViewModel : ViewModelBase
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private DateTimeOffset? _fromDate;

    [ObservableProperty]
    private DateTimeOffset? _toDate;

    [ObservableProperty]
    private ReportStats? _stats;

    [ObservableProperty]
    private bool _isLoading;

    public ReportsViewModel()
    {
        _apiService = new ApiService();
        
        // Default to last 30 days
        _toDate = DateTimeOffset.Now;
        _fromDate = DateTimeOffset.Now.AddDays(-30);
        
        _ = LoadStatsAsync();
    }

    partial void OnFromDateChanged(DateTimeOffset? value) => _ = LoadStatsAsync();
    partial void OnToDateChanged(DateTimeOffset? value) => _ = LoadStatsAsync();

    [RelayCommand]
    private async Task LoadStatsAsync()
    {
        IsLoading = true;
        try
        {
            Stats = await _apiService.GetReportStatsAsync(
                FromDate?.DateTime, 
                ToDate?.DateTime);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportToExcelAsync()
    {
        if (IsLoading) return;
        
        IsLoading = true;
        try
        {
            var content = await _apiService.ExportReportToExcelAsync(FromDate?.DateTime, ToDate?.DateTime);
            
            if (content == null || content.Length == 0)
            {
                await AlertService.ShowAlert("Export Error", "Failed to generate export file or no data available.");
                return;
            }

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var storage = desktop.MainWindow?.StorageProvider;
                if (storage != null)
                {
                    var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Save Excel Report",
                        DefaultExtension = ".xlsx",
                        FileTypeChoices = new[] { new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx" } } },
                        SuggestedFileName = $"BookingReport_{DateTime.Now:yyyyMMdd}.xlsx"
                    });

                    if (file != null)
                    {
                        using var stream = await file.OpenWriteAsync();
                        await stream.WriteAsync(content);
                        await AlertService.ShowAlert("Success", "Report exported successfully!");
                    }
                }
                else
                {
                    await AlertService.ShowAlert("Export Error", "Could not access storage to save the file.");
                }
            }
        }
        catch (Exception ex)
        {
            await AlertService.ShowAlert("Export Error", $"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
