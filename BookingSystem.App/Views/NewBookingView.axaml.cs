using Avalonia.Controls;
using Avalonia.Interactivity;
using BookingSystem.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace BookingSystem.App.Views
{
    public partial class NewBookingView : UserControl
    {
        private readonly ApiService _apiService;
        private List<HallResponse>? _halls;

        public NewBookingView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ = LoadHalls();
        }

        private async Task LoadHalls()
        {
            _halls = await _apiService.GetHallsAsync();
            if (_halls != null)
            {
                var comboBox = this.FindControl<ComboBox>("HallComboBox");
                if (comboBox != null)
                {
                    comboBox.ItemsSource = _halls.Select(h => h.Name).ToList();
                    comboBox.SelectedIndex = 0;
                    UpdateHallInfo(_halls[0]);
                }
            }
        }

        private void HallComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && _halls != null && cb.SelectedIndex >= 0)
            {
                UpdateHallInfo(_halls[cb.SelectedIndex]);
            }
        }

        private void UpdateHallInfo(HallResponse hall)
        {
            var nameText = this.FindControl<TextBlock>("HallNameText");
            if (nameText != null) nameText.Text = hall.Name;

            var capacityText = this.FindControl<TextBlock>("HallCapacityText");
            if (capacityText != null) capacityText.Text = $"{hall.Capacity} People";

            var locationText = this.FindControl<TextBlock>("HallLocationText");
            if (locationText != null) locationText.Text = hall.Location;

            var facilitiesText = this.FindControl<TextBlock>("HallFacilitiesText");
            if (facilitiesText != null) facilitiesText.Text = hall.Facilities;
        }

        private async void SubmitBooking_Click(object? sender, RoutedEventArgs e)
        {
            var hallCombo = this.FindControl<ComboBox>("HallComboBox");
            var datePicker = this.FindControl<DatePicker>("BookingDatePicker");
            var startTimePicker = this.FindControl<TimePicker>("StartTimePicker");
            var endTimePicker = this.FindControl<TimePicker>("EndTimePicker");
            var purposeBox = this.FindControl<TextBox>("PurposeBox");
            var infoBox = this.FindControl<TextBox>("InfoBox");

            if (_halls == null || hallCombo == null || hallCombo.SelectedIndex < 0) return;

            var selectedHall = _halls[hallCombo.SelectedIndex];
            var date = datePicker?.SelectedDate?.DateTime ?? DateTime.Today;
            var start = startTimePicker?.SelectedTime ?? TimeSpan.Zero;
            var end = endTimePicker?.SelectedTime ?? TimeSpan.Zero;

            var startDateTime = date.Date + start;
            var endDateTime = date.Date + end;

            if (endDateTime <= startDateTime)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Error", "End time must be after start time.", MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                await box.ShowAsPopupAsync(this.VisualRoot as Window);
                return;
            }

            var request = new BookingRequest
            {
                HallId = selectedHall.Id,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Purpose = purposeBox?.Text ?? "",
                AdditionalInfo = infoBox?.Text
            };

            try 
            {
                var success = await _apiService.CreateBookingAsync(request);
                
                var window = this.VisualRoot as Window;
                if (window == null) return;

                if (success)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Success", "Booking request submitted successfully!", MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(window);
                    
                    // Clear form
                    if (purposeBox != null) purposeBox.Text = "";
                    if (infoBox != null) infoBox.Text = "";
                }
                else
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error", "Failed to submit booking. There might be a conflict.", MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(window);
                }
            }
            catch (Exception ex)
            {
                var window = this.VisualRoot as Window;
                if (window != null)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error", $"An unexpected error occurred: {ex.Message}", MessageBox.Avalonia.Enums.ButtonEnum.Ok);
                    await box.ShowAsPopupAsync(window);
                }
            }
        }
    }
}
