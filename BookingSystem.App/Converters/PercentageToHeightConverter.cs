using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BookingSystem.App.Converters;

public class PercentageToHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double percentage)
        {
            // Max height is 200
            return (percentage / 100) * 200;
        }
        if (value is int intPercentage)
        {
            return ((double)intPercentage / 100) * 200;
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
