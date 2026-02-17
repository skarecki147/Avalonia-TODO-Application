using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TodoApp.Shared.Converters;

/// <summary>MultiValue: (SelectedNav, IsDarkTheme), Parameter: target nav name. Returns Background brush for nav button.</summary>
public class NavActiveMultiConverter : IMultiValueConverter
{
    private static readonly SolidColorBrush DarkActive = new(Color.Parse("#6c63ff"));
    private static readonly SolidColorBrush DarkInactive = new(Color.Parse("#1a1a3e"));
    private static readonly SolidColorBrush LightActive = new(Color.Parse("#6c63ff"));
    private static readonly SolidColorBrush LightInactive = new(Color.Parse("#e8eaf0"));

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not { Count: >= 1 } || parameter is not string target)
            return DarkInactive;

        var selected = values[0]?.ToString() ?? "";
        var isDark = values.Count < 2 || values[1] is true;
        var active = selected == target;

        if (active)
            return isDark ? DarkActive : LightActive;
        return isDark ? DarkInactive : LightInactive;
    }
}
