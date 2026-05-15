using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BookingSystem.App.Converters;

public class StatusToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status switch
            {
                0 => "Pending",
                1 => "Approved",
                2 => "Rejected",
                3 => "Cancelled",
                _ => "Unknown"
            };
        }
        return "Unknown";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
