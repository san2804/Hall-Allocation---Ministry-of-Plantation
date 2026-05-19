using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace BookingSystem.App.Converters;

public class EqualityConverter : IValueConverter
{
    public object? TargetValue { get; set; }
    public object? TrueValue { get; set; }
    public object? FalseValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool result;
        var target = TargetValue ?? parameter;

        if (value == null && target == null) result = true;
        else if (value == null || target == null) result = false;
        else result = value.ToString() == target.ToString();

        if (TrueValue != null || FalseValue != null)
        {
            return result ? TrueValue : FalseValue;
        }

        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
