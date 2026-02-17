using System.Globalization;
using Avalonia.Data.Converters;

namespace TodoApp.Shared.Converters;

public class PasswordCharConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool showPassword && showPassword)
            return (char)0;
        return '\u25cf'; // ●
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
