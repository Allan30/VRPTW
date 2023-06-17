using System.Globalization;
using System.Windows;
using System;
using System.Windows.Data;

namespace VRPTW.UI.Converters;

public sealed class InvertBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        bool bValue = false;
        if (value is bool boolean)
        {
            bValue = !boolean;
        }
        else if (value is bool?)
        {
            bool? tmp = (bool?)value;
            bValue = !tmp ?? false;
        }
        return (bValue) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }
        else
        {
            return false;
        }
    }
}
