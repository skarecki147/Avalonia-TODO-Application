using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using TodoApp.Core.Enums;

namespace TodoApp.Shared.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TodoPriority priority)
        {
            return priority switch
            {
                TodoPriority.High => new SolidColorBrush(Color.Parse("#f44336")),
                TodoPriority.Medium => new SolidColorBrush(Color.Parse("#ff9800")),
                TodoPriority.Low => new SolidColorBrush(Color.Parse("#4caf50")),
                _ => new SolidColorBrush(Color.Parse("#9e9e9e"))
            };
        }
        return new SolidColorBrush(Color.Parse("#9e9e9e"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
