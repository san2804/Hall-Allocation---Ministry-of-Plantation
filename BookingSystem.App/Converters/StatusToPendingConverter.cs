using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BookingSystem.App.Converters;

public class StatusToPendingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status == 0; // 0 is Pending
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
